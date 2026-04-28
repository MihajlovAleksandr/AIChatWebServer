namespace AIChatWebServer.Services.Interfaces.Notifications
{
    public interface INotificationTokenGetter
    {
        Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetNotificationTokensAsync(
            Guid[] userIds,
            CancellationToken cancellationToken = default);
    }
}
