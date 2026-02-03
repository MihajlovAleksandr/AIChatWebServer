using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class EntryCodeService(IVerificationCodeService verificationCodeService, IEntryTokenFactory entryTokenFactory) : IEntryCodeService
    {                             
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService
            ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly IEntryTokenFactory _entryTokenFactory = entryTokenFactory
            ?? throw new ArgumentNullException(nameof(entryTokenFactory));
        private const string CodeType = "entry_token_code";

        public async Task<string> GenerateAsync(Guid userId, CancellationToken ct = default)
        {
            return _entryTokenFactory.Create(userId, await _verificationCodeService.GenerateAsync(userId, CodeType, ct));
        }

        public async Task VerifyAsync(Guid userId, string code, CancellationToken ct = default)
        {
            await _verificationCodeService.VerifyAsync(userId, CodeType, code, ct);
        }
    }
}
