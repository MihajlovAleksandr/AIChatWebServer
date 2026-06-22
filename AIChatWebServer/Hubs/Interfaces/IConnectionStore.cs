namespace AIChatWebServer.Hubs.Interfaces
{
    public interface IConnectionStore
    {
        bool Add(Guid clientId, string connectionId);
        bool Remove(Guid clientId, string connectionId);
        IReadOnlyList<string>? RemoveAll(Guid clientId);
        IReadOnlyCollection<string> GetConnections(Guid clientId);
        IReadOnlyCollection<string> GetConnections(IEnumerable<Guid>? clientIds);
    }
}
