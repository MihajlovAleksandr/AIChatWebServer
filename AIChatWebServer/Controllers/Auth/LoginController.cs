using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/login")]
    public sealed class LoginController(
        IAuthLoginService loginService,
        IConnectionService connectionService,
        IWorkTokenFactory workTokenFactory,
        IRegistrationTokenFactory registrationTokenFactory,
        IResponseMapper<UserBan, BanResponse> banMapper)
        : ControllerBase
    {
        private readonly IAuthLoginService _loginService = loginService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IRegistrationTokenFactory _registrationTokenFactory = registrationTokenFactory;
        private readonly IResponseMapper<UserBan, BanResponse> _banMapper = banMapper;

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
    }
}
