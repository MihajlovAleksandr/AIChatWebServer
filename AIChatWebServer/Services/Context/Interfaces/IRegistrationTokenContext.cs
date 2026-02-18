using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IRegistrationTokenContext : ITokenContext
    {
        Guid ConnectionId { get; }
        RegistrationState RegistrationState { get; }
    }
}
