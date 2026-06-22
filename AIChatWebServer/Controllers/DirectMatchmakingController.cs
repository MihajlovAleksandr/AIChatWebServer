using AIChatWebServer.DTO.Request;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Connections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/matchmaking/direct")]
    public class DirectMatchmakingController(
        IConnectionValidator connectionValidator,
        IDirectMatchmakingService directMatchmakingService,
        IChatGroupNotifier notifier) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IDirectMatchmakingService _directMatchmakingService = directMatchmakingService;
        private readonly IChatGroupNotifier _notifier = notifier;

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

            ct = CancellationToken.None;

            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId, 
                workTokenContext.UserId, 
                clientContext.Device, ct);

            var result = await _directMatchmakingService.MatchUserAsync(matchmakingRequest.ChatType,
                workTokenContext.UserId,
                matchmakingRequest.ChatMatchPredicate,
                matchmakingRequest.ChatName, ct);

            if (result != null)
                _ = _notifier.ChatCreated(result.ChatId, null, ct);
            else
                _ = _notifier.ChatSearchingStatusUpdated(workTokenContext.UserId, workTokenContext.ConnectionId, true);

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
        public async Task<IActionResult> CancelSearch(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device, ct);

            await _directMatchmakingService.CancelSearch(workTokenContext.UserId, ct);

            _ = _notifier.ChatSearchingStatusUpdated(workTokenContext.UserId, workTokenContext.ConnectionId, false);

            return Ok();
        }
    }
}
