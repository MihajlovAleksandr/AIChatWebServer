using AIChatWebServer.Models.Connection;

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
            Guid defaultUserId = default,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AIChatWebServer.Models.Connection.ConnectionInfo>> GetAllUserConnectionsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> VerifyConnectionAsync(
            Guid id,
            Guid userId,
            string device,
            CancellationToken cancellationToken = default);

        Task<AIChatWebServer.Models.Connection.ConnectionInfo?> RemoveConnectionAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> SetLastConnectionAsync(
            Guid connectionId,
            bool isOnline,
            CancellationToken cancellationToken = default);

        Task<int[]> GetConnectionCountAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<DateTime?> GetLastUserOnlineAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task UpdateConnectionAsync(
            Guid connectionId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task DeleteUnknownConnectionAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
