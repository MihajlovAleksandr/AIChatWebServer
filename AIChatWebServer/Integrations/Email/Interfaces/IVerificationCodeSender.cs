namespace AIChatWebServer.Integrations.Email.Interfaces
{
    public interface IVerificationCodeSender
    {
        Task SendAsync(string recipientEmail, string code, string localization, CancellationToken ct = default);
    }
}
