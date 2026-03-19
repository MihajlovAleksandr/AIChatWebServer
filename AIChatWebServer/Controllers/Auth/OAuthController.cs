using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces;
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
        IRegionGetter regionGetter)
        : ControllerBase
    {
        private readonly IAuthOAuthService _oauthService = oauthService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IRegistrationTokenFactory _registrationTokenFactory = registrationTokenFactory;
        private readonly IOAuthValidator _oAuthValidator = oAuthValidator;
        private readonly IRegionGetter _regionGetter = regionGetter;

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth(
            [FromBody] GoogleTokenRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(context.Device))
                throw new DeviceMissingException();

            if (string.IsNullOrWhiteSpace(context.LanguageCode))
                throw new LanguageMissingException();

            if (string.IsNullOrWhiteSpace(context.IpAddress))
                throw new IpMissingException();

            OAuthUser oauthUser =
                    await _oAuthValidator.ValidateAsync(request.Token);
            try
            {
                User user =
                    await _oauthService.LoginGoogleAsync(
                        oauthUser.Email,
                        oauthUser.Id,
                        ct);
                return await SuccessLoginAsync(
                    user,
                    context.Device,
                    ct);
            }
            catch (UserNotFoundException)
            {
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