using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAIMessageSender
    {
        Task<string> SendAsync(Guid chatId, AIModel model, TokenOperation operation, string prompt, IEnumerable<AIMessage>? messages = null, CancellationToken ct = default);
    }
}
