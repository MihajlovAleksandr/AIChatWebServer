using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Messages;

namespace AIChatWebServer.Services.Implementations.Messages
{
    public class MessageOrchestrator(
        IMessageService messageService,
        IBackgroundJobService backgroundJobService,
        IChatService chatService,
        IMessageProcessorFactory processorFactory,
        IMessageNotifier notifier,
        ILogger<MessageOrchestrator> logger) : IMessageOrchestrator
    {
        private readonly IMessageService _messageService = messageService;
        private readonly IChatService _chatService = chatService;
        private readonly IBackgroundJobService _backgroundJobService = backgroundJobService;
        private readonly IMessageProcessorFactory _processorFactory = processorFactory;
        private readonly IMessageNotifier _notifier = notifier;
        private readonly ILogger<MessageOrchestrator> _logger = logger;

        public async Task<MessageContext> SendMessageAsync(
            Guid messageId,
            Guid chatId,
            Guid userId,
            Guid connectionId,
            string text,
            Guid? uploadSessionId,
            IReadOnlyCollection<MessageReply> replies,
            CancellationToken ct)
        {
            var chat = await _chatService.GetById(chatId, ct);

            var messageContext = await _messageService.CreateAsync(
                messageId,
                chat,
                userId,
                text,
                uploadSessionId,
                replies,
                ct);

            await _notifier.MessageSent(messageContext.Message, connectionId, ct);

            var processor = _processorFactory.Create(chat.Type);

            if (processor != null)
            {
                var message = messageContext.Message;
                var chatCopy = chat;

                _backgroundJobService.FireAndForget(async (serviceProvider, jobCt) =>
                {
                    var processorFactory = serviceProvider.GetRequiredService<IMessageProcessorFactory>();
                    var messageService = serviceProvider.GetRequiredService<IMessageService>();
                    var notifier = serviceProvider.GetRequiredService<IMessageNotifier>();

                    var processor = processorFactory.Create(chatCopy.Type);

                    var result = await processor.ProcessAsync(message, chatCopy, jobCt);

                    if (result == null)
                        return;

                    var aiMessage = await messageService.CreateAsync(
                        Guid.NewGuid(),
                        chatCopy,
                        result.SenderId,
                        result.Text,
                        null,
                        Array.Empty<MessageReply>(),
                        jobCt);

                    await notifier.MessageSent(aiMessage.Message, null, CancellationToken.None);
                }, "ProcessAIResponse", error =>
                {
                    _logger.LogError(error, "Failed to process AI response for message {MessageId}", message.Id);
                });
            }

            return messageContext;
        }

        public async Task<MessageContext> EditTextAsync(
                Guid messageId,
                Guid userId,
                Guid connectionId,
                string text,
                CancellationToken ct)
        {
            await _messageService.EditText(messageId, text, userId, ct);

            var message = await _messageService.GetById(messageId, userId, ct);

            await _notifier.MessageEdited(message.Message, connectionId, ct);

            return message;
        }

        public async Task EditStatusAsync(
            IReadOnlyCollection<Guid> messageIds,
            Guid chatId,
            Guid userId,
            Guid connectionId,
            MessageStatus status,
            CancellationToken ct)
        {
            await _messageService.EditMessagesStatus(
                messageIds,
                chatId,
                status,
                userId,
                ct);

            await _notifier.MessageStatusUpdated(
                messageIds,
                userId,
                connectionId,
                chatId,
                status,
                ct);
        }
    }
}