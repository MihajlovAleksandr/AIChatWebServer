namespace AIChatWebServer.DTO.Response
{
    public class ConnectionResponse(Guid id, Guid userId, string device, DateTime? lastConnection)
    {
        public Guid Id { get; init; } = id;
        public Guid UserId { get; init; } = userId;
        public string Device { get; init; } = device;
        public DateTime? LastConnection { get; init; } = lastConnection;
    }
}
