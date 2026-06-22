using AIChatWebServer.Hubs.Interfaces;
using System.Collections.Concurrent;

namespace AIChatWebServer.Hubs.Implementations
{
    public class ConnectionStore : IConnectionStore
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _connections = new();

        public bool Add(Guid clientId, string connectionId)
        {
            var connections = new ConcurrentDictionary<string, byte>();
            var isNew = _connections.TryAdd(clientId, connections);

            if (!isNew)
                connections = _connections[clientId];

            connections.TryAdd(connectionId, 0);
            return isNew;
        }

        public bool Remove(Guid clientId, string connectionId)
        {
            if (_connections.TryGetValue(clientId, out var connections))
            {
                connections.TryRemove(connectionId, out _);

                if (connections.IsEmpty)
                {
                    _connections.TryRemove(clientId, out _);
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyCollection<string> GetConnections(Guid clientId)
        {
            if (_connections.TryGetValue(clientId, out var connections))
                return connections.Keys.ToList();

            return [];
        }

        public IReadOnlyCollection<string> GetConnections(IEnumerable<Guid>? clientIds)
        {
            if (clientIds == null || !clientIds.Any())
                return [];

            var result = new List<string>();

            foreach (var id in clientIds)
            {
                if (_connections.TryGetValue(id, out var connections))
                    result.AddRange(connections.Keys);
            }

            return result;
        }

        public IReadOnlyList<string>? RemoveAll(Guid clientId)
        {
            if(_connections.TryRemove(clientId, out ConcurrentDictionary<string, byte>? connectionIds))
            {
                return connectionIds.Keys.ToList();
            }
            return null;
        }
    }
}
