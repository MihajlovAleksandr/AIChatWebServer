using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Utils.AI;
using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class AIService(IAIMessageDispatcherFactory dispatcherFactory, IAIMessageRepository aiMessageRepository, IDialogAnalysisParser dialogAnalysisParser, IAIMessageSender sender) : IAIService
    {
        private readonly IAIMessageRepository _aiMessageRepository = aiMessageRepository;
        private readonly IDialogAnalysisParser _dialogAnalysisParser = dialogAnalysisParser;
        private readonly IAIMessageSender _sender = sender;
        private readonly IAIMessageDispatcherFactory _dispatcherFactory = dispatcherFactory;

        public async Task<string> SendMessageAsync(
            Guid chatId,
            AIModel model,
            string prompt,
            string content,
            CancellationToken ct = default)
        {
            var history = await _aiMessageRepository.GetByChatId(chatId, ct);

            var dispatcher = _dispatcherFactory.Create(chatId);

            dispatcher.LoadFromHistory(history);

            AIMessage message = await _aiMessageRepository.Add(
                chatId, AIMessageRole.User,
                AIMessageType.Message,
                content,
                ct);

            await dispatcher.AddMessage(message, ct);

            var contextMessages = dispatcher
                .GetCompressedMessages()
                .Concat(dispatcher.GetMessages())
                .ToList();

            var response = await _sender.SendAsync(
                chatId,
                model,
                TokenOperation.SendMessage,
                prompt,
                contextMessages,
                ct);

            var assistantMessage = await _aiMessageRepository.Add(
                chatId,
                AIMessageRole.Assistant,
                AIMessageType.Message,
                response,
                ct);

            await dispatcher.AddMessage(assistantMessage, ct);

            return response;
        }

        public async Task<string> TranslateAsync(Guid chatId, AIModel model, string langCode, TranslateStyle style, string message)
        {
            return await _sender.SendAsync(chatId, model, TokenOperation.Translate,
                AIPrompts.Translate.IncrementInputs(langCode, style),
                [new AIMessage(chatId, AIMessageRole.User, AIMessageType.Message, message)]);
        }

        public async Task<string> CopmressMessageAsync(Guid chatId, AIModel model, string message)
        {
            return await _sender.SendAsync(chatId, model, TokenOperation.CompressMessage, 
                AIPrompts.CompressMessage.ToString(), 
                [new AIMessage(chatId, AIMessageRole.User, AIMessageType.Message, message)]);
        }

        public async Task<string> CompressDialogAsync(Guid chatId, AIModel model, IEnumerable<Message> messages)
        {
            return _dialogAnalysisParser.ReplaceWithGuids(await _sender.SendAsync(chatId, model, TokenOperation.CompressDialog,
                AIPrompts.CompressDialog.ToString(),
                [new AIMessage(chatId, AIMessageRole.User, AIMessageType.Message, _dialogAnalysisParser.CreateText(messages))]));
        }
    }
}
