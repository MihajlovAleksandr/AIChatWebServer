using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConnectionInfo = AIChatWebServer.Models.Connection.ConnectionInfo;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/user")]
    public sealed class UserController(IConnectionService connectionService, IUserService userService,
        IConnectionValidator connectionValidator, 
        ICollectionResponseMapper<ConnectionInfo, ConnectionResponse> connectionResponseMapper,
        IMapper<UserDataRequest, UserData, UserDataResponse> userDataMapper,
        IRequestMapper<PreferenceRequest, Preference> preferenceMapper,
        IResponseMapper<User, UserResponse> userMapper,
        IResponseMapper<UserInfo, UserInfoResponse> userInfoMapper)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IUserService _userService = userService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly ICollectionResponseMapper<ConnectionInfo, ConnectionResponse> _connectionResponseMapper = connectionResponseMapper;
        private readonly IMapper<UserDataRequest, UserData, UserDataResponse> _userDataMapper = userDataMapper;
        private readonly IRequestMapper<PreferenceRequest, Preference> _preferenceMapper = preferenceMapper;
        private readonly IResponseMapper<User, UserResponse> _userMapper = userMapper;
        private readonly IResponseMapper<UserInfo, UserInfoResponse> _userInfoMapper = userInfoMapper;

        [Authorize]
        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            return Ok(_connectionResponseMapper.ToResponse(await _connectionService.GetAllUserConnectionsAsync(tokenContext.UserId, cancellationToken)));
        }

        [Authorize]
        [HttpPut("userdata")]
        public async Task<IActionResult> UpdateUserData(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices]IClientContext clientContext,
            [FromBody] UserDataRequest userDataRequest,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            UserData userData = _userDataMapper.ToModel(userDataRequest);
            await _userService.UpdateUserData(tokenContext.UserId, userData, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpPut("preference")]
        public async Task<IActionResult> UpdatePreference(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] PreferenceRequest preferenceRequest,
            CancellationToken cancellationToken)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, cancellationToken);

            Preference preference = _preferenceMapper.ToModel(preferenceRequest);
            await _userService.UpdatePreference(tokenContext.UserId, preference, cancellationToken);

            return Ok();
        }

        [Authorize]
        [HttpGet("premium")]
        public async Task<IActionResult> GetPremium(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext, 
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, ct);

            return Ok(await _userService.IsPremium(tokenContext.UserId, ct));
        }

        [HttpGet("{userId}/userdata")]
        public async Task<IActionResult> GetUserData([FromRoute] Guid userId, CancellationToken ct)
        {
            UserInfo info = await _userService.GetUserInfo(userId, ct);

            return Ok(_userInfoMapper.ToResponse(info));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);
            User user = await _userService.GetByIdAsync(workTokenContext.UserId, ct);
            return Ok(_userMapper.ToResponse(user));
        }

        [Authorize]
        [HttpPut("secret")]
        public async Task<IActionResult> UpdateSecret(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] UpdateAuthIdentitySecretRequest request,
            CancellationToken ct)
        {
            string newAuthProvider = request.NewProviderCode ?? request.CurrentProviderCode;
            await _userService.UpdateSecret(workTokenContext.UserId, null, request.CurrentProviderCode, request.CurrentSecret, newAuthProvider, request.NewSecret, ct);
            return Ok();
        }
    }
}
