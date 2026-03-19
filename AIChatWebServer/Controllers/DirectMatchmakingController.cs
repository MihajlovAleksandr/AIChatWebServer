using AIChatWebServer.DTO.Request;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/matchmaking/direct")]
    public class DirectMatchmakingController(
        IConnectionValidator connectionValidator,
        IDirectMatchmakingService directMatchmakingService) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IDirectMatchmakingService _directMatchmakingService = directMatchmakingService;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SearchChat(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] MatchmakingRequest matchmakingRequest,
            CancellationToken ct)
        {
            if (matchmakingRequest.ChatType != ChatType.Human
                && matchmakingRequest.ChatType != ChatType.Random)
                throw new ChatTypeNotSupportedException(matchmakingRequest.ChatType);

            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId, 
                workTokenContext.UserId, 
                clientContext.Device, ct);

            await _directMatchmakingService.MatchUserAsync(matchmakingRequest.ChatType,
                workTokenContext.UserId,
                matchmakingRequest.ChatMatchPredicate,
                matchmakingRequest.ChatName, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> IsSearching(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device, ct);

            return Ok(await _directMatchmakingService.IsSearching(workTokenContext.UserId, ct));
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> CancelSearch([FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device, ct);

            await _directMatchmakingService.CancelSearch(workTokenContext.UserId, ct);

            return Ok();
        }
    }
}
