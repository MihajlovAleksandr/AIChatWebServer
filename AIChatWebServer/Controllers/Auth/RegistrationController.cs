using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Implementations;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;
using AIChatWebServer.Utils.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/register")]
    public sealed class RegistrationController(
        IAuthRegistrationService registrationService,
        IConnectionService connectionService,
        IRegistrationTokenFactory registrationTokenFactory,
        IWorkTokenFactory workTokenFactory,
        IEmailVerificationService emailVerificationService,
        IRegionGetter regionGetter,
        IRequestMapper<UserDataRequest, UserData> userDataMapper,
        IRequestMapper<PreferenceRequest, Preference> preferenceMapper)
        : ControllerBase
    {
        private readonly IAuthRegistrationService _registrationService = registrationService;
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IRegistrationTokenFactory _registrationTokenFactory = registrationTokenFactory;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IEmailVerificationService _emailVerificationService = emailVerificationService;
        private readonly IRegionGetter _regionGetter = regionGetter;
        private readonly IRequestMapper<UserDataRequest, UserData> _userDataMapper = userDataMapper;
        private readonly IRequestMapper<PreferenceRequest, Preference> _preferenceMapper = preferenceMapper;

        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (request == null)
                return BadRequest(ApiError.Create(CommonErrors.RequestBodyEmpty));

            if (string.IsNullOrWhiteSpace(context.Device))
                return BadRequest(ApiError.Create(CommonErrors.DeviceMissing));

            if (string.IsNullOrWhiteSpace(context.LanguageCode))
                return BadRequest(ApiError.Create(CommonErrors.LanguageMissing));

            if (string.IsNullOrWhiteSpace(context.IpAddress))
                return BadRequest(ApiError.Create(CommonErrors.IpMissing));

            if (string.IsNullOrWhiteSpace(request.Identifier))
                return BadRequest(ApiError.Create(LoginErrors.IdentifierRequired));

            if (string.IsNullOrWhiteSpace(request.Secret))
                return BadRequest(ApiError.Create(LoginErrors.SecretRequired));

            if (string.IsNullOrWhiteSpace(request.IdentityProviderCode))
                return BadRequest(ApiError.Create(LoginErrors.ProviderRequired));

            try
            {
                Guid userId =
                    await _registrationService.RegisterAsync(
                        request.Identifier.Trim(),
                        request.Secret,
                        request.IdentityProviderCode.Trim(),
                        _regionGetter.GetCountryCode(context.IpAddress),
                        context.LanguageCode,
                        ct);

                Guid connectionId =
                    await _connectionService.AddConnectionAsync(
                        context.Device,
                        userId,
                        ct);

                await _emailVerificationService.GenerateAsync(
                    request.Identifier.Trim(),
                    userId,
                    context.LanguageCode,
                    ct);

                return NextStep(
                    userId,
                    connectionId,
                    RegistrationState.Created);
            }
            catch (UserAlreadyExistsException)
            {
                return Conflict(ApiError.Create(RegisterErrors.UserAlreadyExists));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CommonErrors.InternalError));
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail(
            VerificationCodeRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            if (request == null)
                return BadRequest(ApiError.Create(CommonErrors.RequestBodyEmpty));

            if (!await ValidateStepAsync(context,RegistrationState.Created, ct))
                return Unauthorized(ApiError.Create(RegisterErrors.InvalidStep));

            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest(ApiError.Create(RegisterErrors.VerificationCodeRequired));

            try
            {
                await _emailVerificationService.VerifyAsync(
                    context.UserId,
                    request.Code,
                    ct);

                await _registrationService.MarkEmailVerifiedAsync(
                    context.UserId,
                    ct);

                return NextStep(
                    context.UserId,
                    context.ConnectionId,
                    RegistrationState.EmailVerified);
            }
            catch (InvalidVerificationCodeException)
            {
                return Unauthorized(ApiError.Create(RegisterErrors.InvalidCode));
            }
            catch (VerificationCodeExpiredException)
            {
                return BadRequest(ApiError.Create(RegisterErrors.CodeExpired));
            }
            catch (VerificationCodeAttemptsExceededException)
            {
                return Forbid();
            }
            catch (VerificationCodeNotFoundException)
            {
                return NotFound(ApiError.Create(RegisterErrors.CodeNotFound));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(CommonErrors.InternalError));
            }
        }

        [HttpPost("userdata")]
        public async Task<IActionResult> AddUserData(
            UserDataRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            if (request == null)
                return BadRequest(ApiError.Create(CommonErrors.RequestBodyEmpty));

            if (!await ValidateStepAsync(context, RegistrationState.EmailVerified, ct))
                return Unauthorized(ApiError.Create(RegisterErrors.InvalidStep));

            try
            {
                UserData model =
                    _userDataMapper.ToModel(request);

                await _registrationService.AddUserDataAsync(
                    context.UserId,
                    model,
                    ct);

                return NextStep(
                    context.UserId,
                    context.ConnectionId,
                    RegistrationState.UserDataCompleted);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(RegisterErrors.UserDataSaveFailed));
            }
        }

        [HttpPost("preference")]
        public async Task<IActionResult> AddPreference(
            PreferenceRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            if (request == null)
                return BadRequest(ApiError.Create(CommonErrors.RequestBodyEmpty));

            if (!await ValidateStepAsync(context, RegistrationState.UserDataCompleted, ct))
                return Unauthorized(ApiError.Create(RegisterErrors.InvalidStep));

            try
            {
                Preference pref =
                    _preferenceMapper.ToModel(request);

                await _registrationService.AddPreferenceAsync(
                    context.UserId,
                    pref,
                    ct);

                await _registrationService.CompleteRegistrationAsync(
                    context.UserId,
                    ct);

                return Ok(
                    _workTokenFactory.Create(
                        context.UserId,
                        context.ConnectionId));
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiError.Create(RegisterErrors.PreferenceSaveFailed));
            }
        }

        private async Task<bool> ValidateStepAsync(
            IRegistrationTokenContext context,
            RegistrationState expected,
            CancellationToken ct)
        {
            RegistrationState realState =
                await _registrationService.GetRegistrationStateAsync(
                    context.UserId,
                    ct);

            if (realState != context.RegistrationState)
                return false;

            if (context.RegistrationState != expected)
                return false;

            if (realState != expected)
                return false;

            return true;
        }

        private IActionResult NextStep(
            Guid userId,
            Guid connectionId,
            RegistrationState state)
        {
            return Ok(new RegisterResponse(
                state,
                _registrationTokenFactory.Create(
                    userId,
                    connectionId,
                    state)));
        }
    }
}
