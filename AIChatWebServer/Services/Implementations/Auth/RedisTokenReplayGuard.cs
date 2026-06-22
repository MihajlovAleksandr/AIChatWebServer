using AIChatWebServer.Services.Interfaces.Auth;
using AIChatWebServer.Services.Interfaces.Utils;
using StackExchange.Redis;

namespace AIChatWebServer.Services.Implementations.Auth
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
