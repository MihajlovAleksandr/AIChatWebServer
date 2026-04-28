using AIChatWebServer.Integrations.AI;

namespace AIChatWebServer.Models.AI
{
    public sealed class AISettingsModel
    {
        public Guid Id { get; }
        public Guid ChatId { get; }
        public string? CustomPrompt { get; }
        public AIModel Model { get; }
        public DateTime LastUpdate { get; }

        public AISettingsModel(
            Guid id,
            Guid chatId,
            string? customPrompt,
            AIModel model,
            DateTime lastUpdate)
        {
            Id = id;
            ChatId = chatId;
            CustomPrompt = customPrompt;
            Model = model;
            LastUpdate = lastUpdate;
        }
    }
}