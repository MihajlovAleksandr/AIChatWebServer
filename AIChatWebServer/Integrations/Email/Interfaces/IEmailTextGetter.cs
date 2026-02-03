using AIChatWebServer.Integrations.Email.DTO;

namespace AIChatWebServer.Integrations.Email.Interfaces
{
    public interface IEmailTextGetter
    {
        EmailMessageRequest GetEmail(string localization, string subject);
    }
}
