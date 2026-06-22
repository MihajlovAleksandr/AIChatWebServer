using AIChatWebServer.Integrations.AI.DTO;

namespace AIChatWebServer.Integrations.AI.Interfaces
{
    public interface IAIController
    {
        Task<AIMessageResponse?> SendMessageAsync(
            IEnumerable<AIMessageRequest> messages,
            string model,
            bool isSystemPrompt = false);
    }

}
