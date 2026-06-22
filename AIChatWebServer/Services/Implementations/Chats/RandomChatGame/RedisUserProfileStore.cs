using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using StackExchange.Redis;
using System.Text.Json;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public sealed class RedisUserProfileStore(
        IConnectionMultiplexer redis) : IUserProfileStore
    {
        private readonly IDatabase _db = redis.GetDatabase();

        private static string BuildKey(Guid chatId)
            => $"user_profile:{chatId}";

        public async Task CreateAsync(
            Guid chatId,
            UserProfile profile,
            TimeSpan? ttl = null,
            CancellationToken ct = default)
        {
            var key = BuildKey(chatId);
            var value = JsonSerializer.Serialize(profile);

            await _db.StringSetAsync(
                key: key,
                value: value,
                expiry: ttl,
                when: When.Always
            );
        }

        public async Task<UserProfile?> GetAsync(
            Guid chatId,
            CancellationToken ct = default)
        {
            var key = BuildKey(chatId);

            string? value = await _db.StringGetAsync(key);

            if (string.IsNullOrEmpty(value))
                return null;

            return JsonSerializer.Deserialize<UserProfile>(value);
        }

        public async Task<bool> DeleteAsync(
            Guid chatId,
            CancellationToken ct = default)
        {
            var key = BuildKey(chatId);

            return await _db.KeyDeleteAsync(key);
        }
    }
}