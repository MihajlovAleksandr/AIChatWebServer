using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using System.Data;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class ConnectionRepository(
        IUserRepository userRepository,
        ILogger<ConnectionRepository> logger) :
        BaseRepository,
        IConnectionRepository
    {
        private readonly ILogger<ConnectionRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task<Guid> AddConnectionAsync(
            string device,
            Guid userId,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Adding new connection for device: {Device}",
                    device);

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.AddConnection,
                        connection);

                command.Parameters.AddWithValue("@Device", device);
                command.Parameters.AddWithValue("@UserId", userId);

                var result =
                    await command.ExecuteScalarAsync(ct);

                var id = (Guid)result!;

                _logger.LogInformation(
                    "New connection added with Id={ConnectionId}",
                    id);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to add connection for device: {Device}",
                    device);

                throw;
            }
        }
        
        public async Task<AIChatWebServer.Models.Connection.ConnectionInfo?> GetConnectionInfoAsync(
            Guid connectionId,
            Guid defaultUserId = default,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Retrieving connection info for ConnectionId={ConnectionId}",
                    connectionId);

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.GetConnectionInfo,
                        connection);

                command.Parameters.AddWithValue("@ConnectionId", connectionId);

                await using var reader =
                    await command.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var info =
                        new AIChatWebServer.Models.Connection.ConnectionInfo(
                            reader.GetGuid("id"),
                            reader.IsDBNull("user_id")
                                ? defaultUserId
                                : reader.GetGuid("user_id"),
                            reader.GetString("device"),
                            reader.IsDBNull("last_connection")
                                ? null
                                : reader.GetDateTime("last_connection"));

                    _logger.LogInformation(
                        "Retrieved connection info for ConnectionId={ConnectionId}",
                        connectionId);

                    return info;
                }

                _logger.LogWarning(
                    "ConnectionId={ConnectionId} not found",
                    connectionId);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve connection info for ConnectionId={ConnectionId}",
                    connectionId);

                throw;
            }
        }

        public async Task<IReadOnlyList<AIChatWebServer.Models.Connection.ConnectionInfo>>
            GetAllUserConnectionsAsync(
                Guid userId,
                CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Retrieving all connections for UserId={UserId}",
                    userId);

                var connections =
                    new List<AIChatWebServer.Models.Connection.ConnectionInfo>();

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.GetAllUserConnections,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);

                await using var reader =
                    await command.ExecuteReaderAsync(ct);

                while (await reader.ReadAsync(ct))
                {
                    connections.Add(
                        new AIChatWebServer.Models.Connection.ConnectionInfo(
                            reader.GetGuid("id"),
                            reader.GetGuid("user_id"),
                            reader.GetString("device"),
                            reader.IsDBNull("last_connection")
                                ? null
                                : reader.GetDateTime("last_connection")));
                }

                _logger.LogInformation(
                    "Retrieved {Count} connections for UserId={UserId}",
                    connections.Count,
                    userId);

                return connections;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve connections for UserId={UserId}",
                    userId);

                throw;
            }
        }

        public async Task<bool> VerifyConnectionAsync(
            Guid id,
            Guid userId,
            string device,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Verifying connection Id={Id} for UserId={UserId}",
                    id,
                    userId);

                var userBan =
                    await _userRepository
                        .GetUserBanByIdAsync(userId, ct);

                if (userBan != null && userBan.IsActual())
                {
                    _logger.LogWarning(
                        "UserId={UserId} is banned until {Until}",
                        userId,
                        userBan.BannedUntil);

                    throw new UserBannedException(userBan);
                }

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.VerifyConnection,
                        connection);

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Device", device);

                var result =
                    await command.ExecuteScalarAsync(ct);

                var count =
                    result is null
                        ? 0
                        : Convert.ToInt64(result);

                var verified = count == 1;

                _logger.LogInformation(
                    "Connection verification Id={Id}: {Verified}",
                    id,
                    verified);

                return verified;
            }
            catch (UserBannedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to verify connection Id={Id}",
                    id);

                throw;
            }
        }


        public async Task<AIChatWebServer.Models.Connection.ConnectionInfo?> RemoveConnectionAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                var info =
                    await GetConnectionInfoAsync(id, default, ct);

                if (info == null)
                {
                    _logger.LogWarning(
                        "No connection found for Id={Id}",
                        id);

                    return null;
                }

                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.RemoveConnection,
                        connection);

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync(ct);

                _logger.LogInformation(
                    "Removed connection Id={Id}",
                    id);

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to remove connection Id={Id}",
                    id);

                throw;
            }
        }

        public async Task<bool> SetLastConnectionAsync(
            Guid connectionId,
            bool isOnline,
            CancellationToken ct = default)
        {
            var query =
                isOnline
                    ? ConnectionQueries.SetLastConnectionOnline
                    : ConnectionQueries.SetLastConnectionOffline;

            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@ConnectionId", connectionId);

                var affected =
                    await command.ExecuteNonQueryAsync(ct);

                var success = affected > 0;

                _logger.LogInformation(
                    "Set last connection Id={Id} to {Status}",
                    connectionId,
                    isOnline ? "online" : "offline");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to set last connection Id={Id}",
                    connectionId);

                throw;
            }
        }

        public async Task<int[]> GetConnectionCountAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.GetConnectionCount,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);

                await using var reader =
                    await command.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var total =
                        (int)reader.GetInt64("devices_count");

                    var online =
                        (int)reader.GetInt64("online_devices_count");

                    _logger.LogInformation(
                        "Connection count UserId={UserId}: {Total}/{Online}",
                        userId,
                        total,
                        online);

                    return [total, online];
                }

                return [-1, -1];
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get connection count UserId={UserId}",
                    userId);

                throw;
            }
        }

        public async Task DeleteUnknownConnectionAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.DeleteUnknownConnection,
                        connection);

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync(ct);

                _logger.LogInformation(
                    "Deleted unknown connection Id={Id}",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete unknown connection Id={Id}",
                    id);

                throw;
            }
        }

        public async Task UpdateConnectionAsync(
            Guid connectionId,
            Guid userId,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.UpdateConnection,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Id", connectionId);

                await command.ExecuteNonQueryAsync(ct);

                _logger.LogInformation(
                    "Updated connection Id={Id} to UserId={UserId}",
                    connectionId,
                    userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update connection Id={Id}",
                    connectionId);

                throw;
            }
        }

        public async Task<DateTime?> GetLastUserOnlineAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            try
            {
                await using var connection =
                    await GetConnectionAsync(ct);

                await using var command =
                    new NpgsqlCommand(
                        ConnectionQueries.GetLastUserOnline,
                        connection);

                command.Parameters.AddWithValue("@UserId", userId);

                var result =
                    await command.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                {
                    _logger.LogInformation(
                        "User {UserId} currently online",
                        userId);

                    return null;
                }

                var lastOnline = (DateTime)result;

                _logger.LogInformation(
                    "User {UserId} last online at {Time}",
                    userId,
                    lastOnline);

                return lastOnline;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get last online UserId={UserId}",
                    userId);

                throw;
            }
        }
    }
}
