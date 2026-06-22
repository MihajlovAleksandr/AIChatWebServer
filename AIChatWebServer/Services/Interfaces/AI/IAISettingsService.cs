using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAISettingsService
    {
        Task<AISettingsModel> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default);

        Task<AISettingsModel> CreateOrUpdate(
            Guid userId,
            Guid chatId,
            AIModel model,
            string? customPrompt,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<AIModel>> GetAvibleModels(
            Guid userId,
            CancellationToken ct = default);
    }
}