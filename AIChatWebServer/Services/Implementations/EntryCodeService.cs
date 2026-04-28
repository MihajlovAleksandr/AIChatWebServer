using AIChatWebServer.Repositories.Models;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class EntryCodeService(IVerificationCodeService verificationCodeService, IEntryTokenFactory entryTokenFactory) : IEntryCodeService
    {                             
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService
            ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly IEntryTokenFactory _entryTokenFactory = entryTokenFactory
            ?? throw new ArgumentNullException(nameof(entryTokenFactory));
        private const string CodeType = "entry_token_code";

        public async Task<string> GenerateAsync(Guid connectionId, Guid userId, CancellationToken ct = default)
        {
            return _entryTokenFactory.Create(userId, await _verificationCodeService.GenerateAsync(userId, connectionId, CodeType, ct));
        }

        public async Task<VerificationCodeRecord> VerifyAsync(Guid userId, string code, CancellationToken ct = default)
        {
            return await _verificationCodeService.VerifyAsync(userId, CodeType, code, ct);
        }

        public async Task DeleteAsync(Guid userId, CancellationToken ct = default)
        {
            await _verificationCodeService.DeleteAsync(userId, CodeType, ct);
        }
    }
}
