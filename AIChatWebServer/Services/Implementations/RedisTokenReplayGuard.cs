using AIChatWebServer.Services.Interfaces;
using StackExchange.Redis;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class RedisTokenReplayGuard(
        IConnectionMultiplexer redis,
        IHasher hasher) : ITokenReplayGuard
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private readonly IHasher _hasher = hasher;

        public async Task<bool> TryMarkAsUsedAsync(
            string token,
            DateTime expiresAtUtc,
            CancellationToken ct = default)
        {
            var ttl = expiresAtUtc - DateTime.UtcNow;
            if (ttl <= TimeSpan.Zero)
                return false;

            var key = "google_token_used:" + _hasher.Hash(token);

            return await _db.StringSetAsync(
                key: key,
                value: "1",
                expiry: ttl,
                when: When.NotExists
            );
        }
    }
}
