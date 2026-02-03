using AIChatWebServer.Services.Interfaces;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace AIChatWebServer.Services.Implementations
{
    public class RedisTokenReplayGuard : ITokenReplayGuard
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisTokenReplayGuard(IConnectionMultiplexer redis)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _db = _redis.GetDatabase();
        }

        public async Task<bool> TryMarkAsUsedAsync(string token, DateTime expiresAtUtc, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var ttl = expiresAtUtc - DateTime.UtcNow;
            if (ttl <= TimeSpan.Zero)
                return false;

            var key = "google_token_used:" + Sha256(token);

            var wasSet = await _db.StringSetAsync(
                key: key,
                value: "1",
                expiry: ttl,
                when: When.NotExists
            );

            return wasSet;
        }

        private static string Sha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
