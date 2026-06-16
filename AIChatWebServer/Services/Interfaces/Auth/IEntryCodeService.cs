using AIChatWebServer.Repositories.Models;

namespace AIChatWebServer.Services.Interfaces.Auth
{
    public interface IEntryCodeService
    {
        Task<string> GenerateAsync(Guid connectionId, Guid userId, CancellationToken ct = default);
        Task<VerificationCodeRecord> VerifyAsync(Guid userId, string code, CancellationToken ct = default);
        Task DeleteAsync(Guid userId, CancellationToken ct = default);
    }
}
