using AIChatWebServer.Models.Exceptions.Implementations.Notification;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class NotificationService(
        INotificationRepository repository,
        ILogger<NotificationService> logger) : INotificationService, INotificationTokenGetter
    {
        private readonly INotificationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        private readonly ILogger<NotificationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task UpdateSettingsAsync(
            Guid userId,
            NotificationSettings settings,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Updating notification settings for UserId={UserId}",
                userId);

            await _repository.UpdateAsync(
                userId,
                settings,
                cancellationToken);
        }

        public async Task<NotificationSettings> GetSettingsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Retrieving notification settings for UserId={UserId}",
                userId);

            return await _repository.GetAsync(
                userId,
                cancellationToken) ?? throw new NotificationSettingsNotFoundException(userId);
        }

        public async Task UpdateNotificationTokenAsync(
            Guid connectionId,
            string token,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Updating notification token for ConnectionId={ConnectionId}",
                connectionId);

            await _repository.UpdateNotificationTokenAsync(
                connectionId,
                token,
                cancellationToken);
        }

        public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>>
            GetNotificationTokensAsync(
                Guid[] userIds,
                CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Retrieving notification tokens for {UserCount} users",
                userIds.Length);

            return await _repository.GetNotificationTokensAsync(
                userIds,
                cancellationToken);
        }
    }
}
