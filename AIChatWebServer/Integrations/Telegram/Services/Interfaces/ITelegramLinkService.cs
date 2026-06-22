using AIChatWebServer.Integrations.Telegram.Models;

namespace AIChatWebServer.Integrations.Telegram.Services.Interfaces
{
    public interface ITelegramLinkService
    {
        Task<string> GenerateAsync(Guid userId, CancellationToken ct);
        Task<UserWithRegionContext> BindTelegramAsync(long tgId, string token, string language, CancellationToken ct);
    }
}
