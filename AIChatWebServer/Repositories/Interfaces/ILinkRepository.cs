using AIChatWebServer.Models.Links;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface ILinkRepository
    {
        Task<Guid> CreateAsync(
            string tokenHash,
            LinkType type,
            string payloadJson,
            Guid createdBy,
            DateTime? expiresAt,
            int maxUses,
            CancellationToken ct = default);

        Task<Link?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<Link?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken ct = default);

        Task<bool> TryIncrementUsageAsync(
            Guid id,
            CancellationToken ct = default);

        Task RevokeAsync(
            Guid id,
            CancellationToken ct = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken ct = default);

        Task<IReadOnlyList<Link>> GetActiveByCreatorAsync(
            Guid creatorId,
            CancellationToken ct = default);
    }
}
