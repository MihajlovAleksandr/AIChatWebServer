using AIChatWebServer.Models.Notification;
using AIChatWebServer.Models.Exceptions.Implementations.Notification;
using AIChatWebServer.Repositories.Constants;
using AIChatWebServer.Repositories.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace AIChatWebServer.Repositories.Implementations
{
    public sealed class NotificationRepository(
        ILogger<NotificationRepository> logger, IConfiguration configuration) : BaseRepository(configuration), INotificationRepository
    {
        private readonly ILogger<NotificationRepository> _logger =
            logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task UpdateAsync(
            Guid userId,
            NotificationSettings settings,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(settings);

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            try
            {
                _logger.LogInformation(
                    "Updating email notification setting for UserId={UserId} to {Value}",
                    userId,
                    settings.EmailNotificationsEnabled);

                await using var connection = await GetConnectionAsync(cancellationToken);
                await using var command = new NpgsqlCommand(
                    NotificationQueries.UpdateNotifications,
                    connection);

                command.Parameters.AddWithValue(
                    "@EmailNotifications",
                    settings.EmailNotificationsEnabled);

                command.Parameters.AddWithValue(
                    "@UserId",
                    userId);

                var affected = await command.ExecuteNonQueryAsync(cancellationToken);

                if (affected == 0)
                {
                    _logger.LogWarning(
                        "Notification settings not found for UserId={UserId}",
                        userId);

                    throw new NotificationSettingsNotFoundException(userId);
                }

                _logger.LogInformation(
                    "Email notification setting successfully updated for UserId={UserId}",
                    userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update email notifications for UserId={UserId}",
                    userId);

                throw;
            }
        }

        public async Task<NotificationSettings?> GetAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            try
            {
                _logger.LogInformation(
                    "Retrieving email notification setting for UserId={UserId}",
                    userId);

                await using var connection = await GetConnectionAsync(cancellationToken);
                await using var command = new NpgsqlCommand(
                    NotificationQueries.GetNotifications,
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);

                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (result == null || result == DBNull.Value)
                {
                    _logger.LogInformation(
                        "No notification settings found for UserId={UserId}",
                        userId);

                    return null;
                }

                var enabled = Convert.ToBoolean(result);

                var settings = new NotificationSettings(enabled);

                _logger.LogInformation(
                    "Email notification setting for UserId={UserId}: {Enabled}",
                    userId,
                    enabled);

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve email notifications for UserId={UserId}",
                    userId);

                throw;
            }
        }

        public async Task UpdateNotificationTokenAsync(
            Guid connectionId,
            string token,
            CancellationToken cancellationToken = default)
        {
            if (connectionId == Guid.Empty)
                throw new ArgumentException("ConnectionId cannot be empty.", nameof(connectionId));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            try
            {
                _logger.LogInformation(
                    "Updating notification token for ConnectionId={ConnectionId}",
                    connectionId);

                await using var connection = await GetConnectionAsync(cancellationToken);
                await using var command = new NpgsqlCommand(
                    NotificationQueries.UpdateNotificationToken,
                    connection);

                command.Parameters.AddWithValue("@NotificationToken", token);
                command.Parameters.AddWithValue("@Id", connectionId);

                var affected = await command.ExecuteNonQueryAsync(cancellationToken);

                if (affected == 0)
                {
                    _logger.LogWarning(
                        "Notification token not found for ConnectionId={ConnectionId}",
                        connectionId);

                    throw new NotificationTokenNotFoundException(connectionId);
                }

                _logger.LogInformation(
                    "Notification token successfully updated for ConnectionId={ConnectionId}",
                    connectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update notification token for ConnectionId={ConnectionId}",
                    connectionId);

                throw;
            }
        }

        public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>>
            GetNotificationTokensAsync(
                Guid[] userIds,
                CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<Guid, IReadOnlyCollection<string>>();

            if (userIds == null || userIds.Length == 0)
            {
                _logger.LogInformation(
                    "No userIds provided to retrieve notification tokens.");

                return result;
            }

            try
            {
                _logger.LogInformation(
                    "Retrieving notification tokens for {UserCount} users",
                    userIds.Length);

                await using var connection = await GetConnectionAsync(cancellationToken);
                await using var command = new NpgsqlCommand(
                    NotificationQueries.GetNotificationTokens,
                    connection);

                command.Parameters.AddWithValue(
                    "@ids",
                    NpgsqlDbType.Array | NpgsqlDbType.Uuid,
                    userIds);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);

                var temp = new Dictionary<Guid, List<string>>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    var userId = reader.GetGuid(0);

                    if (reader.IsDBNull(1))
                        continue;

                    var token = reader.GetString(1);

                    if (!temp.TryGetValue(userId, out var list))
                    {
                        list = new List<string>();
                        temp[userId] = list;
                    }

                    list.Add(token);
                }

                foreach (var userId in userIds)
                {
                    if (!temp.ContainsKey(userId))
                        temp[userId] = new List<string>();
                }

                foreach (var pair in temp)
                    result[pair.Key] = pair.Value.AsReadOnly();

                _logger.LogInformation(
                    "Retrieved notification tokens for {UserCount} users",
                    result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to retrieve notification tokens for users");

                throw;
            }
        }
    }
}
