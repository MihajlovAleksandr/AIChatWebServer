namespace AIChatWebServer.Services.Interfaces.Connections
{
    public interface IConnectionService
    {
        Task<Guid> AddConnectionAsync(
            string device,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Models.Connection.ConnectionInfo> GetConnectionInfoAsync(
            Guid connectionId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Models.Connection.ConnectionInfo>>
            GetAllUserConnectionsAsync(
                Guid userId,
                CancellationToken cancellationToken = default);

        Task<Models.Connection.ConnectionInfo> RemoveConnectionAsync(
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
