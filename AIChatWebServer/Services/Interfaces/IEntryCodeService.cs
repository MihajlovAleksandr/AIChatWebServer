namespace AIChatWebServer.Services.Interfaces
{
    public interface IEntryCodeService
    {
        Task<string> GenerateAsync(Guid userId, CancellationToken ct = default);
        Task VerifyAsync(Guid userId, string code, CancellationToken ct = default);
    }
}
