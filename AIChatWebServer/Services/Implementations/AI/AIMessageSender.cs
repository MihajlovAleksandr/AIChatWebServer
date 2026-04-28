using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Integrations.AI.DTO;
using AIChatWebServer.Integrations.AI.Interfaces;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class AIMessageSender(
        IAIControllerFactory aIControllerFactory,
        IAIMessageRepository aiMessageRepository) : IAIMessageSender
    {
        private readonly IAIControllerFactory _aIControllerFactory = aIControllerFactory;
        private readonly IAIMessageRepository _aiMessageRepository = aiMessageRepository;

        public async Task<string> SendAsync(
            Guid chatId,
            AIModel model,
            TokenOperation operation,
            string prompt,
            IEnumerable<AIMessage>? messages = null,
            CancellationToken ct = default)
        {
            AIMessageResponse response = await SendMessage(model, prompt, messages);
            await _aiMessageRepository.UseTokens(chatId, response.TotalTokensUsed, model, operation, ct);
            return response.Answer;
        }

        private async Task<AIMessageResponse> SendMessage(
            AIModel model,
            string prompt,
            IEnumerable<AIMessage>? messages = null)
        {
            messages ??= [];

            List<AIMessageRequest> messageList = messages
                .Select(m => new AIMessageRequest(m))
                .ToList();

            messageList.Insert(0, new AIMessageRequest(prompt, AIMessageRole.System.ToString()));

            return await SendMessageWithRetryAsync(model, messageList);
        }

        private async Task<AIMessageResponse> SendMessageWithRetryAsync(
            AIModel model,
            IEnumerable<AIMessageRequest> messages)
        {
            string modelName;
            IAIController controller = _aIControllerFactory.Create(model, out modelName);

            AIMessageResponse? response;

            do
            {
                response = await controller.SendMessageAsync(messages, modelName);
            }
            while (response == null);

            return response;
        }
    }
}