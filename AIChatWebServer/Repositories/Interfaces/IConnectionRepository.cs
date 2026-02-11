namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IConnectionRepository
    {
        Task<Guid> AddConnectionAsync(
            string device,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AIChatWebServer.Models.Connection.ConnectionInfo?> GetConnectionInfoAsync(
            Guid connectionId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AIChatWebServer.Models.Connection.ConnectionInfo>> GetAllUserConnectionsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<AIChatWebServer.Models.Connection.ConnectionInfo?> RemoveConnectionAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> SetLastConnectionAsync(
            Guid connectionId,
            bool isOnline,
            CancellationToken cancellationToken = default);

        Task UpdateConnectionAsync(
            Guid connectionId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
