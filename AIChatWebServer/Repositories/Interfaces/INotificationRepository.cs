using AIChatWebServer.Models.Notification;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task UpdateAsync(Guid userId, NotificationSettings settings, CancellationToken cancellationToken = default);
        Task<NotificationSettings?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateNotificationTokenAsync(Guid connectionId, string token, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>>
            GetNotificationTokensAsync(Guid[] userIds, CancellationToken cancellationToken = default);
    }
}
