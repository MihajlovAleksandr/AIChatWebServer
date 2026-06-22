using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Services.Interfaces.Messages
{
    public interface IMessageProcessor
    {
        Task<ProcessorResult?> ProcessAsync(Message message, Chat chat, CancellationToken ct);
    }
}
