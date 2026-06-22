using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Exceptions.Implementations.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Users;

namespace AIChatWebServer.Services.Implementations.AI
{
    public sealed class AISettingsService(IAISettingsRepository repository, IUserAiService userAiService, IUserService userService) : IAISettingsService
    {
        private readonly IAISettingsRepository _repository = repository;
        private readonly IUserService _userService = userService;
        private readonly IUserAiService _userAiService = userAiService;

        public async Task<AISettingsModel> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetByChatId(chatId, cancellationToken)
                ?? throw new ArgumentException("AISettings not configured");
        }

        public async Task<AISettingsModel> CreateOrUpdate(
            Guid userId,
            Guid chatId,
            AIModel model,
            string? customPrompt,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByChatId(chatId, cancellationToken);
            if (!await _userService.IsPremium(userId, cancellationToken))
            {
                if ((existing == null || existing.Model != model) && model != AIModel.Default)
                    if (!await _userAiService.ExistsByUserIdAndModelAsync(userId, model, cancellationToken))
                        throw new AIModelNotAvailableForUserException(userId, model);
            }
            if (existing is null)
            {
                return await _repository.Add(
                    chatId,
                    (int)model,
                    customPrompt,
                    cancellationToken);
            }

            if (existing.CustomPrompt != customPrompt || existing.Model != model)
            {
                await _repository.Update(
                    chatId,
                    (int)model,
                    customPrompt,
                    cancellationToken);
            }

            return new AISettingsModel(
                existing.Id,
                existing.ChatId,
                customPrompt,
                (AIModel)model,
                DateTime.UtcNow
            );
        }

        public async Task<IReadOnlyCollection<AIModel>> GetAvibleModels(Guid userId, CancellationToken ct = default)
        {
            if (await _userService.IsPremium(userId, ct))
                return Enum.GetValues<AIModel>().Distinct().ToList();

            var userModels = await _userAiService.GetAllByUserIdAsync(userId, ct);

            var models = userModels.Select(m => m.Model).ToList();
            models.Add(AIModel.Default);

            return models.Distinct().ToList();
        }
    }
}