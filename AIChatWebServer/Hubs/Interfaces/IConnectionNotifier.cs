namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IConnectionNotifier
    {
        Task ConnectionChanged(Guid connectionId, bool needToUpdateTime, bool isCurrentOnline);
        Task Logout(Guid connectionId, Guid userId, Guid initiatorConnectionId);
        Task ConnectionAdded(Guid connectionId, Guid userId);
        Task EntryCodeUsed(Guid connnectionId);
    }
}
