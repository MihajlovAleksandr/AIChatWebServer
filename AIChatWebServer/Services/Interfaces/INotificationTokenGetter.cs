namespace AIChatWebServer.Services.Interfaces
{
    public interface INotificationTokenGetter
    {
        Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetNotificationTokensAsync(
            Guid[] userIds,
            CancellationToken cancellationToken = default);
    }
}
