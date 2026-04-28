using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Exceptions.Implementations;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using Microsoft.AspNetCore.SignalR;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ChatHub(
        IWorkTokenContext token,
        IClientContext client,
        IChatService chatService,
        IConnectionValidator connectionValidator,
        IConnectionStore connectionStore,
        IGroupService groupService,
        IConnectionNotifier connectionNotifier) : BaseHub
    {
        private readonly IWorkTokenContext _token = token;
        private readonly IClientContext _client = client;
        private readonly IChatService _chatService = chatService;
        private readonly IConnectionValidator _validator = connectionValidator;
        private readonly IConnectionStore _store = connectionStore;
        private readonly IGroupService _groups = groupService;
        private readonly IConnectionNotifier _connectionNotifier = connectionNotifier;

        public override async Task OnConnectedAsync()
        {
            try
            {
                await _validator.ValidateConnectionAsync(
                    _token.ConnectionId,
                    _token.UserId,
                    _client.Device,
                    Context.ConnectionAborted);
            }
            catch (ApiExceptionBase ex)
            {
                throw new HubException(ex.ErrorCode.ToString());
            }

            bool isNewConnection = _store.Add(_token.ConnectionId, Context.ConnectionId);

            await _groups.AddToUserGroupAsync(
                Context.ConnectionId,
                _token.UserId,
                Context.ConnectionAborted);

            var chats = await _chatService.GetByUserId(
                _token.UserId,
                Context.ConnectionAborted);

            foreach (var chat in chats)
            {
                await _groups.AddToChatGroupAsync(
                    Context.ConnectionId,
                    chat.Id,
                    Context.ConnectionAborted);
            }

            await base.OnConnectedAsync();

            if (isNewConnection)
            {
                await _connectionNotifier.ConnectionChanged(_token.ConnectionId, isNewConnection, true);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            bool isLastConnection = _store.Remove(_token.ConnectionId, Context.ConnectionId);

            if (isLastConnection)
            {
                await _connectionNotifier.ConnectionChanged(_token.ConnectionId, isLastConnection, false);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}