namespace AIChatWebServer.Services.Interfaces.Auth
{
    public interface ITokenReplayGuard
    {
        Task<bool> TryMarkAsUsedAsync(string token, DateTime expiresAtUtc, CancellationToken ct = default);
    }
}
