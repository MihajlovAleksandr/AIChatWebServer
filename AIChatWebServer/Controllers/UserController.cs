using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/user")]
    public sealed class UserController(IConnectionService connectionService, IUserService userService,
        IConnectionValidator connectionValidator, 
        ICollectionResponseMapper<Models.Connection.ConnectionInfo, ConnectionResponse> connectionResponseMapper,
        IRequestMapper<UserDataRequest, UserData> userDataRequestMapper, IRequestMapper<PreferenceRequest, Preference> preferenceMapper) : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IUserService _userService = userService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly ICollectionResponseMapper<Models.Connection.ConnectionInfo, ConnectionResponse> _connectionResponseMapper = connectionResponseMapper;
        private readonly IRequestMapper<UserDataRequest, UserData> _userDataRequestMapper = userDataRequestMapper;
        private readonly IRequestMapper<PreferenceRequest, Preference> _preferenceMapper = preferenceMapper;

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

            UserData userData = _userDataRequestMapper.ToModel(userDataRequest);
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
    }
}
