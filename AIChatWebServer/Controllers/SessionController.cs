using AIChatWebServer.DTO.Request;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConnectionInfo = AIChatWebServer.Models.Connection.ConnectionInfo;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/session")]
    public sealed class SessionController(
        IConnectionService connectionService,
        INotificationService notificationService,
        IConnectionValidator connectionValidator,
        IWorkTokenFactory workTokenFactory,
        IDisconnectService disconnectedService)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IDisconnectService _disconnectedService = disconnectedService;

        [HttpGet("connect")]
        public async Task<IActionResult> Connect(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, ct); 

            bool needRefresh =
                tokenContext.ExpiresAtUtc <
                DateTime.UtcNow.AddDays(5);

            if (!needRefresh)
            {
                return Ok(new
                {
                    HubUrl = "/ws/chat",
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
                HubUrl = "/ws/chat",
                Token = newToken,
                RefreshedToken = true,
                tokenContext.ConnectionId,
                tokenContext.UserId,
            });
        }
        [Authorize]
        [HttpDelete("{connectionId?}")]
        public async Task<IActionResult> DeleteConnection(
            Guid? connectionId,
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            Guid effectiveConnectionId = connectionId ?? tokenContext.ConnectionId;

            await _connectionValidator.ValidateConnectionAsync(
                tokenContext.ConnectionId,
                tokenContext.UserId,
                clientContext.Device,
                ct);

            ConnectionInfo connectionInfo = await _connectionService.GetConnectionInfoAsync(
                effectiveConnectionId,
                ct);

            if (connectionInfo.UserId != tokenContext.UserId)
            {
                throw new UserMismatchException(
                    connectionInfo.UserId,
                    tokenContext.UserId);
            }

            await _connectionService.RemoveConnectionAsync(
                effectiveConnectionId,
                ct);
            await _disconnectedService.DisconnectAync(effectiveConnectionId, tokenContext.UserId, tokenContext.ConnectionId, ct);

            return Ok();
        }
       

        [Authorize]
        [HttpPut("token")]
        public async Task<IActionResult> UpdateNotificationToken(
            [FromServices]IWorkTokenContext workTokenContext,
            [FromServices]IClientContext clientContext,
            [FromBody]NotificationTokenRequest notificationTokenRequest,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            await _notificationService.UpdateNotificationTokenAsync(workTokenContext.ConnectionId, notificationTokenRequest.Token, ct);

            return Ok();
        }
    }
}
