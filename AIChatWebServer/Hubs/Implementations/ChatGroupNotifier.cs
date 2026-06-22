using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Services.Interfaces.Notifications;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ChatGroupNotifier(
        IChatEventsDispatcher chatEventsDispatcher,
        IHubGroupDispatcher hubGroupDispatcher,
        IMessageVisibilityPolicy policy,
        IChatService chatService,
        IConnectionService connectionService,
        INotificationFacade notificationFacade,
        IResponseMapper<ChatWithUserContext, ChatResponse> responseMapper) : IChatGroupNotifier
    {
        private readonly IChatEventsDispatcher _chatEventsDispatcher = chatEventsDispatcher;
        private readonly IHubGroupDispatcher _hubGroupDispatcher = hubGroupDispatcher;
        private readonly IMessageVisibilityPolicy _policy = policy;
        private readonly IChatService _chatService = chatService;
        private readonly IResponseMapper<ChatWithUserContext, ChatResponse> _responseMapper = responseMapper;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly INotificationFacade _notificationFacade = notificationFacade;

        public async Task ChatCreated(Guid chatId, Guid userId, Guid? excludedConnectionId, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            ChatResponse response = _responseMapper.ToResponse(
                new ChatWithUserContext(chat, userId));

            await Task.WhenAll(
                _hubGroupDispatcher.AddChatGroup(chatId, ct),
                _chatEventsDispatcher.ChatCreated(userId, excludedConnectionId, response),
                _notificationFacade.SendToUserAsync(chatId, userId, response.Name, NotificationPrompt.NewChat, ct)
            );
        }

        public async Task ChatCreated(Guid chatId, Guid? excludedConnectionId, CancellationToken ct)
        {
            await _hubGroupDispatcher.AddChatGroup(chatId, ct);

            Chat chat = await _chatService.GetById(chatId, ct);

            var tasks = chat.UsersWithData.Keys.Select(userId =>
            {
                ChatResponse response = _responseMapper.ToResponse(
                    new ChatWithUserContext(chat, userId));

                return _chatEventsDispatcher.ChatCreated(
                    userId,
                    excludedConnectionId,
                    response);
            }).ToList();

            tasks.Add(_notificationFacade.SendToChatAsync(chat, NotificationPrompt.NewChat, ct));

            await Task.WhenAll(tasks);
        }

        public Task ChatNameUpdated(Guid chatId, Guid userId, Guid excludedConnectionId, string name, CancellationToken ct)
        {
            return _chatEventsDispatcher.ChatNameUpdated(
                chatId,
                excludedConnectionId,
                new ChatNameUpdatedResponse(chatId, name));
        }

        public async Task ChatEnded(Chat chat, Guid excludedConnectionId, CancellationToken ct)
        {
            DateTime endedTime = chat.EndTime
                ?? throw new ArgumentException("Chat must be ended");

            await Task.WhenAll(
                _chatEventsDispatcher.ChatEnded(
                    chat.Id,
                    excludedConnectionId,
                    new ChatEndedResponse(chat.Id, endedTime)),
                _notificationFacade.SendToChatAsync(chat, NotificationPrompt.EndChat, ct)
            );
        }

        public async Task UserRemoved(Guid chatId, Guid removedUserId, Guid excludedConnectionId, CancellationToken ct)
        {
            await Task.WhenAll(
                _chatEventsDispatcher.ChatUserRemoved(
                    chatId,
                    excludedConnectionId,
                    new ChatUserActionResponse(chatId, removedUserId)),
                _hubGroupDispatcher.RemoveFromChatGroup(
                    removedUserId,
                    chatId,
                    ct)
            );
        }

        public async Task UserAdded(Guid chatId, Guid userId, CancellationToken ct)
        {
            Chat chat = await _chatService.GetById(chatId, ct);

            await _chatEventsDispatcher.ChatUserAdded(
                chatId,
                null,
                new ChatUserActionResponse(chatId, userId));

            await _hubGroupDispatcher.AddToChatGroup(userId, chatId, ct);

            await Task.WhenAll(_chatEventsDispatcher.ChatCreated(
                userId,
                null,
                _responseMapper.ToResponse(new ChatWithUserContext(chat, userId))),
                _notificationFacade.SendToChatAsync(chat, NotificationPrompt.AddUser, ct)
            );
        }

        public Task ChatSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, bool isSearching)
        {
            return _chatEventsDispatcher.ChatSearchingStatusUpdated(
                userId,
                excludedConnectionId,
                new ChatSeachingStatusResponse(isSearching));
        }

        public Task GroupSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, bool isSearching, Guid? chatId)
        {
            return _chatEventsDispatcher.GroupSearchingStatusUpdated(
                userId,
                excludedConnectionId,
                new GroupSeachingStatusResponse(isSearching, chatId));
        }

        public async Task Typing(Guid chatId, Guid userId, bool isTyping)
        {
            Chat chat = await _chatService.GetById(chatId);

            var connections = await _connectionService.GetAllUserConnectionsAsync(userId);

            if (_policy.ShouldIncludeStatuses(chat.Type))
               await _chatEventsDispatcher.Typing(chatId, connections.Select(c => c.Id).ToArray(), new UserTypingResponse(userId, chatId, isTyping));
        }
    }
}