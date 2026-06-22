using AIChatWebServer.Integrations.Email.DTO;
using AIChatWebServer.Integrations.Email.Interfaces;

namespace AIChatWebServer.Integrations.Email.Implementations
{
    public sealed class VerificationCodeSender (IEmailSender emailSender, IEmailTextGetter emailTextGetter) : IVerificationCodeSender
    {
        public async Task SendAsync(string recipientEmail, string code, string localization, CancellationToken ct = default)
        {
            EmailMessageRequest emailMessage = emailTextGetter.GetEmail(localization, "VerificationCode");
            emailMessage.Subject = emailMessage.Subject.Replace("[VERIFICATION_CODE]", code);
            emailMessage.Text = emailMessage.Text.Replace("[VERIFICATION_CODE]", code);
            await emailSender.SendAsync(recipientEmail, emailMessage, ["Assets\\logo.png"], ct);
        }
    }
}
