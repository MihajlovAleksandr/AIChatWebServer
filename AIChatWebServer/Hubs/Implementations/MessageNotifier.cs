using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Services.Interfaces.Notifications;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Hubs.Implementations
{
    public class MessageNotifier(
        IChatService chatService,
        IMessageVisibilityPolicy messageVisibilityPolicy,
        IResponseMapper<MessageContext, MessageResponse> mapper,
        IMessageEventDispatcher messageEventDispatcher, 
        INotificationFacade notificationFacade) : IMessageNotifier
    {
        private readonly IChatService _chatService = chatService;
        private readonly IMessageVisibilityPolicy _messageVisibilityPolicy = messageVisibilityPolicy;
        private readonly IResponseMapper<MessageContext, MessageResponse> _mapper = mapper;
        private readonly IMessageEventDispatcher _messageEventDispatcher = messageEventDispatcher;
        private readonly INotificationFacade _notificationFacade = notificationFacade;

        public async Task MessageSent(Message message, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(message.ChatId, ct);
            bool includeStatuses = _messageVisibilityPolicy.ShouldIncludeStatuses(chat.Type);

            var tasks = chat.UsersWithData.Keys.Select(userId =>
            {
                var response = _mapper.ToResponse(
                    new MessageContext(message, userId, includeStatuses));

                return _messageEventDispatcher.MessageSent(
                    userId,
                    null,
                    response,
                    ct);
            }).ToList();

            tasks.Add(message.Text.Length == 0 
                ? _notificationFacade.SendToChatAsync(chat, message.UserId, NotificationPrompt.NewMessage, ct) 
                : _notificationFacade.SendToChatAsync(chat, message.UserId, message.Text, ct));

            await Task.WhenAll(tasks);
        }

        public Task MessageEdited(Message message, Guid senderConnectionId, CancellationToken ct)
        {
            return _messageEventDispatcher.MessageEdited(
                message.ChatId,
                senderConnectionId,
                new MessageUpdatedResponse(message.Id, message.Text, message.LastUpdate),
                ct);
        }

        public Task MessageDeleted(Guid messageId, Guid chatId, Guid senderConnectionId, CancellationToken ct)
        {
            return _messageEventDispatcher.MessageDeleted(
                chatId,
                senderConnectionId,
                new MessageDeletedResponse(messageId),
                ct);
        }

        public Task MessageFileDeleted(Guid messageId, Guid fileId, Guid chatId, Guid senderConnectionId, CancellationToken ct)
        {
            return _messageEventDispatcher.MessageFileDeleted(
                chatId,
                senderConnectionId,
                new MessageFileDeletedResponse(messageId, fileId),
                ct);
        }

        public async Task MessageStatusUpdated(
            IReadOnlyCollection<Guid> messages,
            Guid senderUserId,
            Guid senderConnectionId,
            Guid chatId,
            MessageStatus status,
            CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);
            bool includeStatuses = _messageVisibilityPolicy.ShouldIncludeStatuses(chat.Type);

            var tasks = chat.UsersWithData.Keys.Select(userId =>
            {
                if (userId == senderUserId)
                {
                    return _messageEventDispatcher.MessageStatusUpdated(
                        userId,
                        senderConnectionId,
                        new MessageStatusUpdatedResponse(messages, senderUserId, chatId, status),
                        ct);
                }

                if (!includeStatuses)
                {
                    return Task.CompletedTask;
                }

                return _messageEventDispatcher.MessageStatusUpdated(
                    userId,
                    null,
                    new MessageStatusUpdatedResponse(messages, senderUserId, chatId, status),
                    ct);
            });

            await Task.WhenAll(tasks);
        }
    }
}