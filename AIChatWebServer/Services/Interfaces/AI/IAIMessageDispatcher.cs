using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAIMessageDispatcher
    {
        void LoadFromHistory(IEnumerable<AIMessage> messages);
        Task AddMessage(AIMessage message, CancellationToken ct = default);
        IReadOnlyList<AIMessage> GetMessages();
        public IReadOnlyList<AIMessage> GetCompressedMessages();
    }
}
