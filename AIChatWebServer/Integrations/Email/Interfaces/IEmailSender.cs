using AIChatWebServer.Integrations.Email.DTO;

namespace AIChatWebServer.Integrations.Email.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(
                    string email,
                    EmailMessageRequest message,
                    string[] imagePaths,
                    CancellationToken cancellationToken = default);
    }
}
