using AIChatWebServer.Integrations.Email.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class EmailVerificationService(IVerificationCodeService verificationCodeService, IVerificationCodeSender verificationCodeSender) : IEmailVerificationService
    {
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService 
            ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly IVerificationCodeSender _verificationCodeSender = verificationCodeSender
            ?? throw new ArgumentNullException(nameof(verificationCodeSender));
        private const string CodeType = "email";

        public async Task GenerateAsync(string email, Guid userId, string langCode, CancellationToken ct = default)
        {
            await _verificationCodeSender.SendAsync(email, await _verificationCodeService.GenerateAsync(userId, CodeType, ct), langCode, ct);
        }

        public async Task VerifyAsync(Guid userId, string code, CancellationToken ct = default)
        {
            await _verificationCodeService.VerifyAsync(userId, CodeType, code, ct);
        }
    }
}
