using AIChatWebServer.Models.Chats;

namespace AIChatWebServer.Services.Interfaces.Messages
{
    public interface IMessageProcessorFactory
    {
        IMessageProcessor? Create(ChatType chatType);
    }
}
