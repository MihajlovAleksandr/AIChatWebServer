using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAIMessageCompressor
    {
        Task<AIMessage> Compress(
            IReadOnlyList<AIMessage> messages,
            CancellationToken cancellationToken = default);
    }
}