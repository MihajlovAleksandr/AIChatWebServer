using AIChatWebServer.DTO.Response;

namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IChatEventsDispatcher
    {
        Task ChatCreated(Guid userId, Guid? excludedConnectionId, ChatResponse response);
        Task ChatDeleted(Guid userId, Guid excludedConnectionId, ChatDeletedResponse response);
        Task ChatEnded(Guid chatId, Guid excludedConnectionId, ChatEndedResponse response);
        Task ChatNameUpdated(Guid chatId, Guid excludedConnectionId, ChatNameUpdatedResponse response);
        Task ChatUserAdded(Guid chatId, Guid? excludedConnectionId, ChatUserActionResponse response);
        Task ChatUserRemoved(Guid chatId, Guid excludedConnectionId, ChatUserActionResponse response);
        Task ChatSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, ChatSeachingStatusResponse response);
        Task GroupSearchingStatusUpdated(Guid userId, Guid excludedConnectionId, GroupSeachingStatusResponse response);
    }
}
