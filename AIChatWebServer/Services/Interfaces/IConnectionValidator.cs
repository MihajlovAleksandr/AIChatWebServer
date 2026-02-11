namespace AIChatWebServer.Services.Interfaces
{
    public interface IConnectionValidator
    {
        Task ValidateConnectionAsync(Guid connectionId, Guid userId, string? device, CancellationToken cancellationToken);
    }
}
