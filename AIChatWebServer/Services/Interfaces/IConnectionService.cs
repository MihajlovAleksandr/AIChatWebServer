namespace AIChatWebServer.Services.Interfaces
{
    public interface IConnectionService
    {
        Task<Guid> AddConnectionAsync(
            string device,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Models.Connection.ConnectionInfo?> GetConnectionInfoAsync(
            Guid connectionId,
            Guid defaultUserId = default,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Models.Connection.ConnectionInfo>>
            GetAllUserConnectionsAsync(
                Guid userId,
                CancellationToken cancellationToken = default);

        Task<bool> VerifyConnectionAsync(
            Guid id,
            Guid userId,
            string device,
            CancellationToken cancellationToken = default);

        Task<Models.Connection.ConnectionInfo?> RemoveConnectionAsync(
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
