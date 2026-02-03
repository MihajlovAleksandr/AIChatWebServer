namespace AIChatWebServer.Services.Interfaces
{
    public interface ITokenReplayGuard
    {
        Task<bool> TryMarkAsUsedAsync(string token, DateTime expiresAtUtc, CancellationToken ct = default);
    }
}
