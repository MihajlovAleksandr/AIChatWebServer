using AIChatWebServer.Integrations.Email.DTO;
using AIChatWebServer.Integrations.Email.Interfaces;

namespace AIChatWebServer.Integrations.Email.Implementations
{
    public class EmailTextGetter : IEmailTextGetter
    {
        public EmailMessageRequest GetEmail(string localization, string subject)
        {
            return new EmailMessageRequest(
                File.ReadAllText($"Assets\\emails\\{localization}\\{subject}\\subject.txt"),
                File.ReadAllText($"Assets\\emails\\{localization}\\{subject}\\text.html"));
        }
    }
}
