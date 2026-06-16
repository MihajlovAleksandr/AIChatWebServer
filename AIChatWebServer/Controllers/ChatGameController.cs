using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Ranks;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;
using AIChatWebServer.Services.Interfaces.Chats.Ranks;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Users;
using AIChatWebServer.Utils.Implementations.Mappers;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/chat/game")]
    public class ChatGameController(IChatGameService chatGameService, IConnectionValidator connectionValidator, IPointsService pointsService, IUserService userService, IResponseMapper<UserInfo, UserInfoResponse> mapper) : ControllerBase
    {
        private readonly IChatGameService _chatGameService = chatGameService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IPointsService _pointsService = pointsService;
        private readonly IUserService _userService = userService;
        private readonly IResponseMapper<UserInfo, UserInfoResponse> _mapper = mapper;

        [Authorize]
        [HttpPost("guess")]
        public async Task<IActionResult> MakeQuessAsync(
            [FromBody] MakeQuessRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _chatGameService.MakeGuessAsync(request.ChatId, request.AiRole, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid chatId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            var session = await _chatGameService.GetByChatIdAsync(chatId, ct);
            
            return Ok(await _chatGameService.GetResultAsync(session.Id, ct));
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> Get(
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1,
            CancellationToken ct = default)
        {
            int offset = (page - 1) * limit;

            List<UserPoints> points = await _pointsService.GetLeaderboardAsync(limit, offset, ct);

            int totalCount = await _pointsService.GetLeaderboardTotalCountAsync(ct);

            int totalPages = (int)Math.Ceiling((double)totalCount / limit);

            var leaderboardItems = new List<LeaderboardItemResponse>();

            foreach (var up in points)
            {
                var userInfo = await _userService.GetUserInfo(up.UserId, ct);
                leaderboardItems.Add(new LeaderboardItemResponse(_mapper.ToResponse(userInfo), up.TotalPoints));
            }

            var response = new PaginationItem<LeaderboardItemResponse>(
                leaderboardItems,
                totalCount,
                page,
                totalPages,
                limit
            );

            return Ok(response);
        }
    }
}
