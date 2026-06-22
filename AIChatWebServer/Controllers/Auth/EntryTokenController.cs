using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Auth;
using AIChatWebServer.Services.Interfaces.Connections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/code")]
    public sealed class EntryTokenController(
        IConnectionService connectionService,
        IConnectionValidator connectionValidator,
        IEntryCodeService entryCodeService,
        IWorkTokenFactory workTokenFactory,
        IConnectionNotifier notifier)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IEntryCodeService _entryCodeService = entryCodeService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;
        private readonly IConnectionNotifier _notifier = notifier;

        [Authorize]
        [HttpGet("generate")]
        public async Task<IActionResult> GenerateEntryCode(
            [FromServices] IWorkTokenContext workContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workContext.ConnectionId,
                workContext.UserId,
                clientContext.Device,
                ct);

            string code =
                await _entryCodeService.GenerateAsync(
                    workContext.ConnectionId,
                    workContext.UserId,
                    ct);

            return Ok(code);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEntryCode(
            [FromServices] IEntryTokenContext entryContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {

            if (string.IsNullOrWhiteSpace(clientContext.Device))
                throw new DeviceMissingException();

            var code = await _entryCodeService.VerifyAsync(
                entryContext.UserId,
                entryContext.Code,
                ct);

            Guid connectionId =
                await _connectionService.AddConnectionAsync(
                    clientContext.Device,
                    entryContext.UserId,
                    ct);

            await _notifier.EntryCodeUsed(code.ConnectionId);
            await _notifier.ConnectionAdded(connectionId, entryContext.UserId);

            return Ok(
                _workTokenFactory.Create(
                    entryContext.UserId,
                    connectionId));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCode(
            [FromServices] IWorkTokenContext workContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workContext.ConnectionId,
                workContext.UserId,
                clientContext.Device,
                ct);

            await _entryCodeService.DeleteAsync(
                workContext.UserId,
                ct);

            return Ok();
        }
    }
}
