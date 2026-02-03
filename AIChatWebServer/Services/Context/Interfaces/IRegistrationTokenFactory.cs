using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface IRegistrationTokenFactory
    {
        string Create(Guid userId, Guid connectionId, RegistrationState registrationState);
    }
}
