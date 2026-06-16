using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Utils.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public class UserProfileGenerator(IUserProfileStore userProfileStore, IAIMessageSender aIMessageSender) : IUserProfileGenerator
    {
        private readonly IUserProfileStore _userProfileStore = userProfileStore;
        private readonly IAIMessageSender _aIMessageSender = aIMessageSender;

        public async Task<UserProfile> GenerateAync(Guid chatId, User user, CancellationToken ct)
        {
            var core = await GenerateWithRetry<CoreDto>(
                chatId,
                AIPrompts.GeneratePersonaCore,
                $"{user.UserData}, language: {user.Language[LanguageContext.Account]}, region: {user.RegionCode}",
                3,
                ct);

            var coreJson = JsonSerializer.Serialize(core);

            var personalityTask = GenerateWithRetry<PersonalityDto>(
                chatId,
                AIPrompts.GeneratePersonaPersonality,
                coreJson,
                3,
                ct);

            var socialTask = GenerateWithRetry<SocialDto>(
                chatId,
                AIPrompts.GeneratePersonaSocial,
                coreJson,
                3,
                ct);

            var knowledgeTask = GenerateWithRetry<KnowledgeDto>(
                chatId,
                AIPrompts.GeneratePersonaKnowledge,
                coreJson,
                3,
                ct);

            await Task.WhenAll(personalityTask, socialTask, knowledgeTask);

            var profile = new UserProfile
            {
                Name = core.Name,
                Age = core.Age,
                Gender = core.Gender,
                Country = core.Country,
                City = core.City,
                Languages = core.Languages,
                Education = core.Education,
                Occupation = core.Occupation,
                IncomeLevel = core.IncomeLevel,
                LivingSituation = core.LivingSituation,

                Interests = personalityTask.Result.Interests,
                Hobbies = personalityTask.Result.Hobbies,
                DailyRoutine = personalityTask.Result.DailyRoutine,
                PersonalityTraits = personalityTask.Result.PersonalityTraits,
                Quirks = personalityTask.Result.Quirks,
                CommunicationStyle = personalityTask.Result.CommunicationStyle,

                SocialContext = socialTask.Result.SocialContext,
                AdaptationToOther = socialTask.Result.AdaptationToOther,

                KnowledgeScope = knowledgeTask.Result.KnowledgeScope,
                Limitations = knowledgeTask.Result.Limitations,
                Opinions = knowledgeTask.Result.Opinions,
                Background = knowledgeTask.Result.Background
            };

            await _userProfileStore.CreateAsync(chatId, profile, ct: ct);

            return profile;
        }

        private async Task<T> GenerateWithRetry<T>(
            Guid chatId,
            AIPrompts prompt,
            string? input,
            int maxRetries,
            CancellationToken ct)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var finalPrompt = input == null
                        ? prompt.ToString()
                        : prompt.IncrementInputs(input);

                    var response = await _aIMessageSender.SendAsync(
                        chatId,
                        AIModel.Default,
                        TokenOperation.GeneratePersona,
                        finalPrompt,
                        ct: ct,
                        isSustemPrompt: true);

                    if (string.IsNullOrWhiteSpace(response))
                        throw new Exception("Empty response");

                    using var doc = JsonDocument.Parse(response);

                    var result = JsonSerializer.Deserialize<T>(response);

                    if (result != null && Validate(result))
                        return result;
                }
                catch
                {
                    if (attempt == maxRetries)
                        break;

                    var delay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt));
                    await Task.Delay(delay, ct);
                }
            }

            throw new Exception($"Failed to generate {typeof(T).Name} after {maxRetries} attempts");
        }
        private static bool Validate<T>(T obj)
        {
            return obj switch
            {
                CoreDto c =>
                    !string.IsNullOrWhiteSpace(c.Name) &&
                    c.Age > 0 &&
                    c.Languages.Count > 0,

                PersonalityDto p =>
                    p.PersonalityTraits.Length >= 2 &&
                    p.Quirks.Length >= 2 &&
                    p.Interests.Length >= 2,

                SocialDto s =>
                    !string.IsNullOrWhiteSpace(s.SocialContext.SocialCircle),

                KnowledgeDto k =>
                    k.KnowledgeScope.StrongAreas.Length >= 1 &&
                    k.Limitations.Length >= 1 &&
                    !string.IsNullOrWhiteSpace(k.Background),

                _ => false
            };
        }
    }

    public sealed class CoreDto
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;
        [JsonPropertyName("age")]
        public int Age { get; init; }
        [JsonPropertyName("gender")]
        public string Gender { get; init; } = default!;
        [JsonPropertyName("country")]
        public string Country { get; init; } = default!;
        [JsonPropertyName("city")]
        public string City { get; init; } = default!;
        [JsonPropertyName("languages")]
        public Dictionary<string, string> Languages { get; init; } = [];
        [JsonPropertyName("education")]
        public string Education { get; init; } = default!;
        [JsonPropertyName("occupation")]
        public string Occupation { get; init; } = default!;
        [JsonPropertyName("incomeLevel")]
        public string IncomeLevel { get; init; } = default!;
        [JsonPropertyName("livingSituation")]
        public string LivingSituation { get; init; } = default!;
    }

    public sealed class PersonalityDto
    {
        [JsonPropertyName("personalityTraits")]
        public string[] PersonalityTraits { get; init; } = [];
        [JsonPropertyName("quirks")]
        public string[] Quirks { get; init; } = [];
        [JsonPropertyName("communicationStyle")]
        public string CommunicationStyle { get; init; } = default!;
        [JsonPropertyName("dailyRoutine")]
        public string DailyRoutine { get; init; } = default!;
        [JsonPropertyName("interests")]
        public string[] Interests { get; init; } = [];
        [JsonPropertyName("hobbies")]
        public string[] Hobbies { get; init; } = [];
    }

    public sealed class SocialDto
    {
        [JsonPropertyName("socialContext")]
        public SocialContext SocialContext { get; init; } = new();
        [JsonPropertyName("adaptationToOther")]
        public AdaptationToOther AdaptationToOther { get; init; } = new();
    }

    public sealed class KnowledgeDto
    {
        [JsonPropertyName("knowledgeScope")]
        public KnowledgeScope KnowledgeScope { get; init; } = new();
        [JsonPropertyName("limitations")]
        public string[] Limitations { get; init; } = [];
        [JsonPropertyName("opinions")]
        public Opinions Opinions { get; init; } = new();
        [JsonPropertyName("background")]
        public string Background { get; init; } = default!;
    }
}
