using AIChatWebServer.DTO.Response;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IChatGroupEventDispatcher
    {
        Task GroupCreated(Guid userId, ChatResponse response);
    }
}
