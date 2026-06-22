using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IAISettingsRepository
    {
        Task<AISettingsModel?> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default);

        Task<AISettingsModel> Add(
            Guid chatId,
            int model,
            string? customPrompt,
            CancellationToken cancellationToken = default);

        Task<bool> Update(
            Guid chatId,
            int model,
            string? customPrompt,
            CancellationToken cancellationToken = default);
    }
}