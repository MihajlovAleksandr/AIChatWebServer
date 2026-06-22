namespace AIChatWebServer.Services.Interfaces.Connections
{
    public interface IConnectionValidator
    {
        Task ValidateConnectionAsync(Guid connectionId, Guid userId, string? device, CancellationToken cancellationToken);
    }
}
