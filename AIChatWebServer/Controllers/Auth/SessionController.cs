using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Tokens.Interfaces;
using AIChatWebServer.Utils.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/session")]
    public sealed class SessionController(
        IConnectionService connectionService,
        IWorkTokenFactory workTokenFactory)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;

        [HttpGet("connect")]
        public async Task<IActionResult> Connect(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(clientContext.Device))
            {
                return BadRequest(
                    ApiError.Create(CommonErrors.DeviceMissing));
            }

            bool verified =
                await _connectionService.VerifyConnectionAsync(
                    tokenContext.ConnectionId,
                    tokenContext.UserId,
                    clientContext.Device,
                    ct);

            if (!verified)
            {
                return Unauthorized(
                    ApiError.Create(SessionErrors.InvalidConnection));
            }

            bool needRefresh =
                tokenContext.ExpiresAtUtc <
                DateTime.UtcNow.AddDays(5);


            if (!needRefresh)
            {
                return Ok(new
                {
                    HubUrl = "/ws/connect",
                    RefreshedToken = false,
                    tokenContext.ConnectionId,
                    tokenContext.UserId
                });
            }

            string newToken =
                _workTokenFactory.Create(
                    tokenContext.UserId,
                    tokenContext.ConnectionId);


            return Ok(new
            {
                HubUrl = "/ws/connect",
                Token = newToken,
                RefreshedToken = true,
                tokenContext.ConnectionId,
                tokenContext.UserId
            });
        }
        [Authorize]
        [HttpDelete("{connectionId}")]
        public async Task<IActionResult> DeleteConnection(Guid connectionId, IWorkTokenContext tokenContext)
        {
            Models.Connection.ConnectionInfo? connectionInfo = await _connectionService.GetConnectionInfoAsync(connectionId);
            if (connectionInfo == null)
            {
                return NotFound(
                    ApiError.Create(SessionErrors.ConnectionNotFound));
            }

            if (connectionInfo.UserId != tokenContext.UserId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    ApiError.Create(SessionErrors.ConnectionForbidden));
            }

            await _connectionService.RemoveConnectionAsync(connectionId);
            return Ok();
        }
    }
}
