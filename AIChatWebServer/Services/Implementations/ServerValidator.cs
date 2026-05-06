using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class ServerValidator(IUserService userService) : IServerValidator
    {
        private readonly IUserService _userService = userService;

        public async Task Validate(Guid userId, Servers server, CancellationToken ct)
        {
            User user = await _userService.GetByIdAsync(userId, ct);
            if(!Enum.TryParse<Servers>(user.Email, out Servers real))
            {
                throw new InvalidServerException(userId);
            }
            if (server != real)
                throw new InvalidServerException(userId);
        }
    }
}
