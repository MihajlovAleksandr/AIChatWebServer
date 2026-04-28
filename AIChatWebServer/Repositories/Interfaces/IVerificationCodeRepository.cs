using AIChatWebServer.Repositories.Models;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task UpsertAsync(
            Guid userId,
            Guid connectionId,
            string type,
            string codeHash,
            DateTime expiresAt,
            CancellationToken cancellationToken = default);

        Task<VerificationCodeRecord?> GetAsync(
            Guid userId,
            string type,
            CancellationToken cancellationToken = default);

        Task IncrementAttemptsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            string type,
            CancellationToken cancellationToken = default);
    }
}
