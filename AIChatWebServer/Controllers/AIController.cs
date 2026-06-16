using AIChatWebServer.DTO.Request;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Connections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController(IAIService aiService, IConnectionValidator connectionValidator) : ControllerBase
    {
        private readonly IAIService _aiService = aiService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;

        [Authorize]
        [HttpPost("translate")]
        public async Task<IActionResult> Translate(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext context,
            [FromBody] TranslateRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.ConnectionId, context.Device, ct);

            var result = await _aiService.TranslateAsync(
                request.ChatId,
                request.Model,
                request.LangCode,
                request.Style,
                request.Message);

            return Ok(new { result });
        }

        [Authorize]
        [HttpPost("compress-message")]
        public async Task<IActionResult> CompressMessage(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext context,
            [FromBody] CompressMessageRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.ConnectionId, context.Device, ct);

            var result = await _aiService.CopmressMessageAsync(
                request.ChatId,
                request.Model,
                request.Message);

            return Ok(new { result });
        }
        [Authorize]
        [HttpPost("compress-dialog")]
        public async Task<IActionResult> CompressDialog(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext context,
            [FromBody] CompressDialogRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.ConnectionId, context.Device, ct);

            var result = await _aiService.CompressDialogAsync(
                request.ChatId,
                request.Model,
                request.Messages);

            return Ok(new { result });
        }
    }
}