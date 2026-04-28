using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAIService
    {
        Task<string> SendMessageAsync(
            Guid chatId,
            AIModel model,
            string prompt,
            string content,
            CancellationToken ct = default);

        Task<string> TranslateAsync(
            Guid chatId,
            AIModel model,
            string langCode,
            TranslateStyle style,
            string message);

        Task<string> CopmressMessageAsync(
            Guid chatId,
            AIModel model,
            string message);

        Task<string> CompressDialogAsync(
            Guid chatId,
            AIModel model,
            IEnumerable<Message> messages);
    }
}