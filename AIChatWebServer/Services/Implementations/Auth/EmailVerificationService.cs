using AIChatWebServer.Integrations.Email.Interfaces;
using AIChatWebServer.Services.Interfaces.Auth;
using AIChatWebServer.Services.Interfaces.Utils;

namespace AIChatWebServer.Services.Implementations.Auth
{
    public sealed class EmailVerificationService(IVerificationCodeService verificationCodeService, IVerificationCodeSender verificationCodeSender) : IEmailVerificationService
    {
        private readonly IVerificationCodeService _verificationCodeService = verificationCodeService 
            ?? throw new ArgumentNullException(nameof(verificationCodeService));
        private readonly IVerificationCodeSender _verificationCodeSender = verificationCodeSender
            ?? throw new ArgumentNullException(nameof(verificationCodeSender));
        private const string CodeType = "email";

        public async Task GenerateAsync(string email, Guid connectionId, Guid userId, string langCode, CancellationToken ct = default)
        {
            string code = await _verificationCodeService.GenerateAsync(userId, connectionId, CodeType, ct);
            Console.WriteLine(code);
            await _verificationCodeSender.SendAsync(email, code, langCode, ct);
        }

        public async Task VerifyAsync(Guid userId, string code, CancellationToken ct = default)
        {
            await _verificationCodeService.VerifyAsync(userId, CodeType, code, ct);
        }
    }
}
