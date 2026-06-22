using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Auth.Register;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Auth;
using AIChatWebServer.Services.Interfaces.Connections;
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
            [FromBody] RegisterRequest request,
            [FromServices] IClientContext context,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(context.Device))
                throw new DeviceMissingException();

            if (string.IsNullOrWhiteSpace(context.LanguageCode))
                throw new LanguageMissingException();

            if (string.IsNullOrWhiteSpace(context.IpAddress))
                throw new IpMissingException();

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
                connectionId,
                userId,
                context.LanguageCode,
                ct);

            return NextStep(
                userId,
                connectionId,
                RegistrationState.Created);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail(
            [FromBody] VerificationCodeRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            await ValidateStepAsync(context, RegistrationState.Created, ct);

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

        [HttpPost("userdata")]
        public async Task<IActionResult> AddUserData(
            [FromBody] UserDataRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            await ValidateStepAsync(context, RegistrationState.EmailVerified, ct);

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

        [HttpPost("preference")]
        public async Task<IActionResult> AddPreference(
            [FromBody] PreferenceRequest request,
            [FromServices] IRegistrationTokenContext context,
            CancellationToken ct)
        {
            await ValidateStepAsync(context, RegistrationState.UserDataCompleted, ct);

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

        private async Task ValidateStepAsync(
            IRegistrationTokenContext context,
            RegistrationState expected,
            CancellationToken ct)
        {
            RegistrationState realState =
                await _registrationService.GetRegistrationStateAsync(
                    context.UserId,
                    ct);

            if (realState != context.RegistrationState
                || context.RegistrationState != expected 
                || realState != expected)
                    throw new InvalidRegisterStepException(context.RegistrationState, expected, realState);
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
