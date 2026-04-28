using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Utils.AI;

namespace AIChatWebServer.Services.Implementations.Messages.Processors
{
    public class RandomMessageProcessor(
        IConfiguration configuration,
        IAIService aIService, 
        IChatGameService chatGameService,
        IUserProfileStore userProfileStore, 
        IQueryClassifier queryClassifier) : IMessageProcessor
    {                                                                                      
        private readonly IAIService _aIService = aIService;
        private readonly IChatGameService _chatGameService = chatGameService;
        private readonly IUserProfileStore _userProfileStore = userProfileStore;
        private readonly IQueryClassifier _queryClassifier = queryClassifier;
        private readonly Guid _aIId = Guid.TryParse(configuration["SystemUsers:AIId"], out Guid id)
            ? id : throw new ArgumentException("AI Id is not valid.");

        public async Task<ProcessorResult?> ProcessAsync(Message message, Chat chat, CancellationToken ct)
        {
            ChatGameSession game = await _chatGameService.GetByChatIdAsync(chat.Id, ct);
            if (game.GetGuesser().UserChatId == chat.UsersWithData[message.UserId].Id && game.GetOpponent().AiRole == AiRole.RealAi)
            {
                QueryTags tags = await _queryClassifier.ClassifyAsync(chat.Id, message.Text, ct);
                UserProfile userProfile = await _userProfileStore.GetAsync(chat.Id, ct)
                    ?? throw new ArgumentException("User Profile not configured");
                string response = await _aIService.SendMessageAsync(
                    chat.Id,
                    AIModel.Default,
                    AIPrompts.RandomChatDefault.IncrementInputs(
                        userProfile.BuildContext(tags)),
                    message.Text,
                    ct: ct);
                return new ProcessorResult(response, _aIId);
            }
            return null;
        }
    }
}
