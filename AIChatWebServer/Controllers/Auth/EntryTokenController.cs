using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
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
        IWorkTokenFactory workTokenFactory)
        : ControllerBase
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IEntryCodeService _entryCodeService = entryCodeService;
        private readonly IWorkTokenFactory _workTokenFactory = workTokenFactory;

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

            await _entryCodeService.VerifyAsync(
                entryContext.UserId,
                entryContext.Code,
                ct);

            Guid connectionId =
                await _connectionService.AddConnectionAsync(
                    clientContext.Device,
                    entryContext.UserId,
                    ct);

            return Ok(
                _workTokenFactory.Create(
                    entryContext.UserId,
                    connectionId));
        }
    }
}
