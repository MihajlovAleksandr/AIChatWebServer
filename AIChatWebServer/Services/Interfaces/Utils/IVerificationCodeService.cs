using AIChatWebServer.Repositories.Models;

namespace AIChatWebServer.Services.Interfaces.Utils
{
    public interface IVerificationCodeService
    {
        Task<string> GenerateAsync(
            Guid userId,
            Guid connectionId,
            string type,
            CancellationToken cancellationToken = default);

        Task<VerificationCodeRecord> VerifyAsync(
            Guid userId,
            string type,
            string code,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid userId,
            string type,
            CancellationToken ct = default);
    }
}
