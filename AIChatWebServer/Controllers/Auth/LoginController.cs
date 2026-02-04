using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;
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
            LoginRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (request == null)
                return BadRequest(
                    ApiError.Create(CommonErrors.RequestBodyEmpty));

            if (string.IsNullOrWhiteSpace(request.Identifier))
                return BadRequest(
                    ApiError.Create(LoginErrors.IdentifierRequired));

            if (string.IsNullOrWhiteSpace(request.Secret))
                return BadRequest(
                    ApiError.Create(LoginErrors.SecretRequired));

            if (string.IsNullOrWhiteSpace(request.IdentityProviderCode))
                return BadRequest(
                    ApiError.Create(LoginErrors.ProviderRequired));

            if (string.IsNullOrWhiteSpace(context.Device))
                return BadRequest(
                    ApiError.Create(CommonErrors.DeviceMissing));
            try
            {
                User? user =
                    await _loginService.LoginAsync(
                        request.Identifier.Trim(),
                        request.Secret,
                        request.IdentityProviderCode.Trim(),
                        ct);

                if (user == null)
                {
                    return Unauthorized(
                        ApiError.Create(LoginErrors.InvalidCredentials));
                }


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
            catch (UserBannedException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    _banMapper.ToResponse(ex.UserBan));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CommonErrors.InternalError));
            }
        }
    }
}
