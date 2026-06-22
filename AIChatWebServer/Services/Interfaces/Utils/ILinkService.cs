using AIChatWebServer.Models.Links;

namespace AIChatWebServer.Services.Interfaces.Utils
{
    public interface ILinkService
    {
        Task<string> CreateAsync(
            LinkType type,
            string payloadJson,
            Guid createdBy,
            DateTime? expiresAt,
            int maxUses,
            CancellationToken ct = default);

        Task<Link> GetActiveAsync(
            string token,
            CancellationToken ct = default);

        Task<Link> ExecuteAsync(
            string token,
            CancellationToken ct = default);

        Task RevokeAsync(
            Guid linkId,
            CancellationToken ct = default);
    }
}