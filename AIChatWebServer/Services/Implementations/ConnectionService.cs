using AIChatWebServer.Models.Exceptions.Implementations.Connection;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class ConnectionService(
        IConnectionRepository connectionRepository,
        ILogger<ConnectionService> logger) : IConnectionService
    {
        private readonly IConnectionRepository _connectionRepository =
                connectionRepository
                ?? throw new ArgumentNullException(nameof(connectionRepository));
        private readonly ILogger<ConnectionService> _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Guid> AddConnectionAsync(
            string device,
            Guid userId,
            CancellationToken ct = default)
        {
            var connectionId =
                await _connectionRepository
                    .AddConnectionAsync(device, userId, ct);

            _logger.LogInformation(
                "Connection {ConnectionId} added for device {Device}.",
                connectionId,
                device);

            return connectionId;
        }

        public async Task<Models.Connection.ConnectionInfo> GetConnectionInfoAsync(
            Guid connectionId,
            CancellationToken ct = default)
        {
            Models.Connection.ConnectionInfo info =
                await _connectionRepository
                    .GetConnectionInfoAsync(
                        connectionId,
                        ct) ?? throw new ConnectionNotFoundException(connectionId);

            _logger.LogInformation(
                "Retrieved connection info for ConnectionId {ConnectionId}.",
                connectionId);

            return info;
        }

        public async Task<IReadOnlyList<Models.Connection.ConnectionInfo>> GetAllUserConnectionsAsync(
                Guid userId,
                CancellationToken ct = default)
        {
            var connections =
                await _connectionRepository
                    .GetAllUserConnectionsAsync(userId, ct);

            _logger.LogInformation(
                "Retrieved {Count} connections for User {UserId}.",
                connections.Count,
                userId);

            return connections;
        }

        public async Task<Models.Connection.ConnectionInfo> RemoveConnectionAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var removed =
                await _connectionRepository
                    .RemoveConnectionAsync(id, ct)
                ?? throw new ConnectionNotFoundException(id);

            _logger.LogInformation(
                "Connection {ConnectionId} removed successfully.",
                id);

            return removed;
        }

        public async Task<bool> SetLastConnectionAsync(
            Guid connectionId,
            bool isOnline,
            CancellationToken ct = default)
        {
            var result =
                await _connectionRepository
                    .SetLastConnectionAsync(
                        connectionId,
                        isOnline,
                        ct);

            if (result)
            {
                _logger.LogInformation(
                    "Updated last connection status for Connection {ConnectionId} to {Status}.",
                    connectionId,
                    isOnline ? "Online" : "Offline");
            }
            else
            {
                _logger.LogWarning(
                    "Failed to update last connection status for Connection {ConnectionId}.",
                    connectionId);
            }

            return result;
        }

        public async Task UpdateConnectionAsync(
            Guid connectionId,
            Guid userId,
            CancellationToken ct = default)
        {
            await _connectionRepository
                .UpdateConnectionAsync(
                    connectionId,
                    userId,
                    ct);

            _logger.LogInformation(
                "Updated Connection {ConnectionId} to User {UserId}.",
                connectionId,
                userId);
        }
    }
}
