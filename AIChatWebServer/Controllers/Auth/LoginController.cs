using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Consts;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Auth;
using AIChatWebServer.Services.Interfaces.Connections;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/login")]
    public sealed class LoginController(
        IAuthLoginService loginService,
        IConnectionService connectionService,
        IWorkTokenFactory workTokenFactory,
        IServerTokenFactory serverTokenFactory,
        IRegistrationTokenFactory registrationTokenFactory)
        : ControllerBase
    {
        private readonly IAuthLoginService _loginService = loginService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IServerTokenFactory _serverTokenFactory = serverTokenFactory;
        private readonly IRegistrationTokenFactory _registrationTokenFactory = registrationTokenFactory;

        [HttpPost]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(context.Device))
                throw new DeviceMissingException();

            User user = await _loginService.LoginAsync(
                request.Identifier.Trim(),
                request.Secret,
                request.IdentityProviderCode.Trim(),
                ct);

            Guid connectionId =
                await _connectionService.AddConnectionAsync(
                    context.Device,
                    user.Id,
                    ct);

            if (user.RegistrationState != RegistrationState.Completed)
            {
                return Ok(new RegisterResponse(
                    user.RegistrationState,
                    _registrationTokenFactory.Create(
                        user.Id,
                        connectionId,
                        user.RegistrationState)));
            }
            return Ok(
                _workTokenFactory.Create(
                    user.Id,
                    connectionId));

        }

        [HttpPost("server")]
        public async Task<IActionResult> LoginServer(
            [FromBody] LoginRequest request,
            CancellationToken ct)
        {
            User user = await _loginService.LoginAsync(
                request.Identifier.Trim(),
                request.Secret,
                request.IdentityProviderCode.Trim(),
                ct);

            return Ok(
                _serverTokenFactory.Create(
                    user.Id,
                    Enum.Parse<Servers>(user.Email)));
        }

    }
}
