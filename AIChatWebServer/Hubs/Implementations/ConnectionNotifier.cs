using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Utils.Interfaces.Mapper;
using ConnectionInfo = AIChatWebServer.Models.Connection.ConnectionInfo;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ConnectionNotifier(
        IConnectionService connectionService,
        IChatService chatService,
        IConnectionEventDispatcher connectionEventDispatcher,
        ICollectionResponseMapper<ConnectionInfo, ConnectionResponse> connectionMapper,
        IResponseMapper<SyncModel, SyncResponse> syncMapper,
        ISyncService syncService) : IConnectionNotifier
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IChatService _chatService = chatService;
        private readonly IConnectionEventDispatcher _connectionEventDispatcher = connectionEventDispatcher;
        private readonly ICollectionResponseMapper<ConnectionInfo, ConnectionResponse> _connectionMapper = connectionMapper;
        private readonly IResponseMapper<SyncModel, SyncResponse> _syncMapper = syncMapper;
        private readonly ISyncService _syncService = syncService;

        public async Task ConnectionChanged(Guid connectionId, bool needToUpdateTime, bool isCurrentOnline)
        {
            ConnectionInfo info = await _connectionService.GetConnectionInfoAsync(connectionId);

            if (needToUpdateTime)
            {
                await _connectionService.SetLastConnectionAsync(
                    connectionId,
                    isCurrentOnline);
            }

            IReadOnlyCollection<ConnectionInfo> connectionInfos =
                await _connectionService.GetAllUserConnectionsAsync(info.UserId);

            DateTime? lastOnline = DateTime.MinValue;

            foreach(ConnectionInfo current in connectionInfos)
            {
                if(current.LastOnline == null)
                {
                    lastOnline = null;
                    break;
                }
                if(current.LastOnline>lastOnline)
                    lastOnline = current.LastOnline;
            }

            if (lastOnline == null)
            {
                await _connectionEventDispatcher.ConnectionChanged(
                    info.UserId,
                    [info.Id],
                    new ConnectionChangedResponse(
                        _connectionMapper.ToResponse(connectionInfos)));
            }
            else
            {
                IReadOnlyCollection<Chat> chats =
                    await _chatService.GetByUserId(info.UserId);

                var tasks = chats.Select(chat =>
                    _connectionEventDispatcher.OnlineStatusChanged(
                        chat.Id,
                        info.Id,
                        new OnlineStatusChangedResponse(info.UserId, lastOnline)
                    ));

                await Task.WhenAll(tasks);
            }

            if (needToUpdateTime && isCurrentOnline)
            {
                SyncModel syncModel = await _syncService.SyncAsync(info.UserId, info.LastOnline);

                await _connectionEventDispatcher.SyncDB(
                    connectionId,
                    _syncMapper.ToResponse(syncModel));
            }
        }

        public async Task ConnectionAdded(Guid connectionId, Guid userId)
        {
            IReadOnlyList<ConnectionInfo> connectionInfos =
                await _connectionService.GetAllUserConnectionsAsync(userId);

            await _connectionEventDispatcher.ConnectionChanged(
                userId,
                [connectionId],
                new ConnectionChangedResponse(
                    _connectionMapper.ToResponse(connectionInfos)));
        }

        public Task EntryCodeUsed(Guid connnectionId)
        {
            return _connectionEventDispatcher.EntryCodeUsed(connnectionId);
        }

        public async Task Logout(Guid connectionId, Guid userId, Guid initiatorConnectionId)
        {
            var tasks = new List<Task>();

            if (connectionId != initiatorConnectionId)
            {
                tasks.Add(_connectionEventDispatcher.Logout(connectionId));
            }

            var connectionInfosTask = _connectionService.GetAllUserConnectionsAsync(userId);

            await Task.WhenAll(tasks.Concat([connectionInfosTask]));

            var connectionInfos = await connectionInfosTask;

            await _connectionEventDispatcher.ConnectionChanged(
                userId,
                [initiatorConnectionId, connectionId],
                new ConnectionChangedResponse(
                    _connectionMapper.ToResponse(connectionInfos)
                )
            );
        }
    }
}