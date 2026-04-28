using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class AISettingsService : IAISettingsService
    {
        private readonly IAISettingsRepository _repository;

        public AISettingsService(IAISettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<AISettingsModel> GetByChatId(
            Guid chatId,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetByChatId(chatId, cancellationToken)
                ?? throw new ArgumentException("AISettings not configured");
        }

        public async Task<AISettingsModel> CreateOrUpdate(
            Guid chatId,
            AIModel model,
            string? customPrompt,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByChatId(chatId, cancellationToken);

            if (existing is null)
            {
                return await _repository.Add(
                    chatId,
                    (int)model,
                    customPrompt,
                    cancellationToken);
            }

            if (existing.CustomPrompt != customPrompt)
            {
                await _repository.UpdatePrompt(
                    chatId,
                    customPrompt,
                    cancellationToken);
            }

            return new AISettingsModel(
                existing.Id,
                existing.ChatId,
                customPrompt,
                (AIModel)model,
                DateTime.UtcNow
            );
        }

        public async Task<AISettingsModel> UpdatePrompt(
            Guid chatId,
            string? customPrompt,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByChatId(chatId, cancellationToken);

            if (existing is null)
            {
                throw new InvalidOperationException("AI settings not found for chat");
            }

            await _repository.UpdatePrompt(
                chatId,
                customPrompt,
                cancellationToken);

            return new AISettingsModel(
                existing.Id,
                existing.ChatId,
                customPrompt,
                existing.Model,
                DateTime.UtcNow
            );
        }
    }
}