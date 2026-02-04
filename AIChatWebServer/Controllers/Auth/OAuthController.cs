using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Utils.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/oauth")]
    public sealed class OAuthController(
        IAuthOAuthService oauthService,
        IConnectionService connectionService,
        IWorkTokenFactory workTokenFactory,
        IRegistrationTokenFactory registrationTokenFactory,
        IOAuthValidator oAuthValidator,
        IRegionGetter regionGetter,
        IResponseMapper<UserBan, BanResponse> banMapper)
        : ControllerBase
    {
        private readonly IAuthOAuthService _oauthService = oauthService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IRegistrationTokenFactory _registrationTokenFactory = registrationTokenFactory;
        private readonly IOAuthValidator _oAuthValidator = oAuthValidator;
        private readonly IRegionGetter _regionGetter = regionGetter;
        private readonly IResponseMapper<UserBan, BanResponse> _banMapper = banMapper;

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth(
            GoogleTokenRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(
                    ApiError.Create(OAuthErrors.TokenInvalid));
            }

            if (string.IsNullOrWhiteSpace(context.Device) ||
                string.IsNullOrWhiteSpace(context.IpAddress) ||
                string.IsNullOrWhiteSpace(context.LanguageCode))
            {
                return BadRequest(
                    ApiError.Create(OAuthErrors.ContextMissing));
            }

            OAuthUser? oauthUser;

            try
            {
                oauthUser =
                    await _oAuthValidator.ValidateAsync(request.Token);
            }
            catch
            {
                return Unauthorized(
                    ApiError.Create(OAuthErrors.TokenInvalid));
            }

            if (oauthUser == null)
            {
                return Unauthorized(
                    ApiError.Create(OAuthErrors.TokenInvalid));
            }
            try
            {
                User? user =
                    await _oauthService.LoginGoogleAsync(
                        oauthUser.Email,
                        oauthUser.Id,
                        ct);

                if (user != null)
                {
                    return await SuccessLoginAsync(
                        user,
                        context.Device!,
                        ct);
                }

                Guid userId =
                    await _oauthService.RegisterGoogleAsync(
                        oauthUser.Email,
                        oauthUser.Id,
                        _regionGetter.GetCountryCode(context.IpAddress!),
                        context.LanguageCode,
                        ct);


                Guid connectionId =
                    await _connectionService.AddConnectionAsync(
                        context.Device!,
                        userId,
                        ct);


                return Ok(
                    new RegisterResponse(RegistrationState.EmailVerified,
                    _registrationTokenFactory.Create(
                        userId,
                        connectionId,
                        RegistrationState.EmailVerified)));
            }
            catch (UserAlreadyExistsException)
            {
                return Conflict(
                    ApiError.Create(OAuthErrors.UserUnauthorized));
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
                    ApiError.Create(OAuthErrors.LoginFailed));
            }
        }

        private async Task<IActionResult> SuccessLoginAsync(
            User user,
            string device,
            CancellationToken ct)
        {
            Guid connectionId =
                await _connectionService.AddConnectionAsync(
                    device,
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