namespace AIChatWebServer.Services.Interfaces
{
    public interface IEmailVerificationService
    {
        Task GenerateAsync(string email, Guid userId, string langCode, CancellationToken ct = default);
        Task VerifyAsync(Guid userId, string code, CancellationToken ct = default);
    }
}
