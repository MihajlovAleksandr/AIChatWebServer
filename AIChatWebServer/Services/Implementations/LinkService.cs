using AIChatWebServer.Models.Exceptions.Implementations.Link;
using AIChatWebServer.Models.Links;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;
using System.Security.Cryptography;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class LinkService(ILinkRepository repository, IHasher hasher) : ILinkService
    {
        private readonly ILinkRepository _repository = repository;
        private readonly IHasher _hasher = hasher;

        public async Task<string> CreateAsync(
            LinkType type,
            string payloadJson,
            Guid createdBy,
            DateTime? expiresAt,
            int maxUses,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new ArgumentException("Payload is required.", nameof(payloadJson));

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxUses);

            await ValidateCreate(createdBy, type);

            string token = GenerateToken();
            string hash = _hasher.Hash(token);

            await _repository.CreateAsync(
                hash,
                type,
                payloadJson,
                createdBy,
                expiresAt,
                maxUses,
                ct);

            return token;
        }

        public async Task<Link> GetActiveAsync(
            string token,
            CancellationToken ct = default)
        {
            string hash = _hasher.Hash(token);

            var link =
                await _repository.GetByTokenHashAsync(hash, ct)
                ?? throw new LinkNotFoundException(token);

            if (!link.IsActive(DateTime.UtcNow))
                throw new LinkNotActiveException(link.Id);

            return link;
        }

        public async Task<Link> ExecuteAsync(
            string token,
            CancellationToken ct = default)
        {
            string hash = _hasher.Hash(token);

            var link =
                await _repository.GetByTokenHashAsync(hash, ct)
                ?? throw new LinkNotFoundException(token);

            if (!link.IsActive(DateTime.UtcNow))
                throw new LinkNotActiveException(link.Id);

            bool incremented =
                await _repository.TryIncrementUsageAsync(link.Id, ct);

            if (!incremented)
                throw new LinkUsageLimitReachedException(link.Id);

            return link;
        }

        public async Task RevokeAsync(
            Guid linkId,
            CancellationToken ct = default)
        {
            await _repository.RevokeAsync(linkId, ct);
        }

        private static string GenerateToken()
        {
            Span<byte> bytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(bytes);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private async Task ValidateCreate(Guid userId, LinkType type)
        {
            var links = await _repository.GetActiveByCreatorAsync(userId);

            if (links.Any(link => link.Type == type))
                throw new LinkAlreadyExistsForTypeException(userId, type);
        }
    }
}