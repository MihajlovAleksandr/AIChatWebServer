using AIChatWebServer.DTO.Request;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Chats.Matchmaking;
using AIChatWebServer.Models.Exceptions.Implementations.Chat.Matchmaking;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Connections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/matchmaking/group")]
    public class GroupMatchmakingController(
        IConnectionValidator connectionValidator,
        IGroupMatchmakingService groupMatchmakingService,
        IChatGroupNotifier notifier
        ) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IGroupMatchmakingService _groupMatchmakingService = groupMatchmakingService;
        private readonly IChatGroupNotifier _notifier = notifier;

        [Authorize]
        [HttpPost("user")]
        public async Task<IActionResult> SearchChat(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] SearchChatRequest searchChatRequest,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device, ct);

            GroupMatchmakingResult? result = 
                await _groupMatchmakingService.MatchUserAsync(
                    workTokenContext.UserId,
                    searchChatRequest.ChatMatchPredicate,
                    searchChatRequest.ChatName, ct) as GroupMatchmakingResult;
            if (result != null)
                _ = _notifier.UserAdded(result.ChatId, result.UserId, ct);
            else
                _ = _notifier.GroupSearchingStatusUpdated(workTokenContext.UserId, workTokenContext.ConnectionId, true, null);

            return Ok();
        }

        [Authorize]
        [HttpPost("chat")]
        public async Task<IActionResult> SearchUser(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromBody] SearchUserRequest searchUserRequest,
            CancellationToken ct)
        {
            if (searchUserRequest.Slots <= 0)
                throw new InvalidSlotsException(searchUserRequest.Slots);

            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device, ct);

            GroupMatchmakingResult? result = await _groupMatchmakingService.MatchChatAsync(
                searchUserRequest.ChatId,
                new StartSearchChatAction(
                    workTokenContext.UserId,
                    searchUserRequest.ChatMatchPredicate,
                    searchUserRequest.Slots), ct) as GroupMatchmakingResult;

            if (result != null)
                _ = _notifier.UserAdded(result.ChatId, result.UserId, ct);
            else
                _ = _notifier.GroupSearchingStatusUpdated(workTokenContext.UserId, workTokenContext.ConnectionId, true, searchUserRequest.ChatId);

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

            return Ok(await _groupMatchmakingService.IsSearching(workTokenContext.UserId, ct));
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

            await _groupMatchmakingService.CancelSearch(workTokenContext.UserId, ct);
            _ = _notifier.GroupSearchingStatusUpdated(workTokenContext.UserId, workTokenContext.ConnectionId, false, null);

            return Ok();
        }
    }
}
