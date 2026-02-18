namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IWorkTokenContext : ITokenContext
    {
        Guid ConnectionId { get; }
    }
}
