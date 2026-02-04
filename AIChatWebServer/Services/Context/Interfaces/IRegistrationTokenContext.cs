using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;

namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface IRegistrationTokenContext : ITokenContext
    {
        Guid ConnectionId { get; }
        RegistrationState RegistrationState { get; }
    }
}
