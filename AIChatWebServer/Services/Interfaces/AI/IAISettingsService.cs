using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAISettingsService
    {
        Task<AISettingsModel> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default);

        Task<AISettingsModel> CreateOrUpdate(
            Guid chatId,
            AIModel model,
            string? customPrompt,
            CancellationToken cancellationToken = default);

        Task<AISettingsModel> UpdatePrompt(
            Guid chatId,
            string? customPrompt,
            CancellationToken cancellationToken = default);
    }
}