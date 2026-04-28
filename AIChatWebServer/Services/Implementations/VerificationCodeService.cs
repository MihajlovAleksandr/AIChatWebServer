using AIChatWebServer.Models.Exceptions.Implementations.Auth.VerificationCode;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Repositories.Models;
using AIChatWebServer.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class VerificationCodeService(
        IVerificationCodeRepository repository,
        IHasher hasher) :
        IVerificationCodeService
    {
        private const int CodeLength = 6;
        private const int MaxAttempts = 5;

        private static readonly TimeSpan CodeTtl =
            TimeSpan.FromMinutes(10);

        private readonly IVerificationCodeRepository _repository =
                repository
                ?? throw new ArgumentNullException(nameof(repository));
        private readonly IHasher _hasher =
                hasher
                ?? throw new ArgumentNullException(nameof(hasher));

        public async Task<string> GenerateAsync(
            Guid userId,
            Guid connectionId,
            string type,
            CancellationToken ct = default)
        {
            var code = GenerateCode();

            var hash = _hasher.Hash(code);

            await _repository.UpsertAsync(
                userId,
                connectionId,
                type,
                hash,
                DateTime.UtcNow.Add(CodeTtl),
                ct);

            return code;
        }

        public async Task<VerificationCodeRecord> VerifyAsync(
            Guid userId,
            string type,
            string code,
            CancellationToken ct = default)
        {
            var record =
                await _repository
                    .GetAsync(userId, type, ct) ?? throw new VerificationCodeNotFoundException(userId, type);

            if (record.ExpiresAt <= DateTime.UtcNow)
                throw new VerificationCodeExpiredException(record.ExpiresAt, userId);

            if (record.Attempts >= MaxAttempts)
                throw new VerificationCodeAttemptsExceededException(userId, record.Attempts, MaxAttempts);

            if (!_hasher.Verify(code, record.CodeHash))
            {
                await _repository
                    .IncrementAttemptsAsync(record.Id, ct);

                throw new InvalidVerificationCodeException(userId);
            }

            await _repository
                .DeleteAsync(record.Id, type, ct);
            return record;
        }

        public async Task DeleteAsync(
            Guid userId,
            string type,
            CancellationToken ct = default)
        {
            var code = await _repository
                    .GetAsync(userId, type, ct) ?? throw new VerificationCodeNotFoundException(userId, type);
            await _repository.DeleteAsync(code.Id, type, ct);
        }

        private static string GenerateCode()
        {
            var bytes =
                RandomNumberGenerator.GetBytes(CodeLength);

            var sb =
                new StringBuilder(CodeLength);

            foreach (var b in bytes)
                sb.Append(b % 10);

            return sb.ToString();
        }
    }
}
