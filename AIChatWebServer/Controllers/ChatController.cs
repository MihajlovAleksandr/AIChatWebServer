using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController(IConnectionValidator connectionValidator, IChatService chatService, IResponseMapper<ChatWithUserContext, ChatResponse> chatResponseMapper) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IChatService _chatService = chatService;
        private readonly IResponseMapper<ChatWithUserContext, ChatResponse> _chatResponseMapper = chatResponseMapper;

        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(Guid chatId,
            [FromServices]IWorkTokenContext workTokenContext,
            [FromServices]IClientContext clientContext, 
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            ChatResponse chatResponse = _chatResponseMapper.ToResponse(new ChatWithUserContext(await _chatService.GetById(chatId, ct), workTokenContext.UserId));

            return Ok(chatResponse);
        }

        [Authorize]
        [HttpPut("{chatId}/name")]
        public async Task<IActionResult> UpdateName(Guid chatId,
            [FromBody] ChatNameRequest chatNameRequest,
            [FromServices]IWorkTokenContext workTokenContext,
            [FromServices]IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            await _chatService.ExecuteAction(chatId, new UpdateNameAction(workTokenContext.UserId, chatNameRequest.Name), ct);

            return Ok();
        }

        [Authorize]
        [HttpPost("{chatId}/end")]
        public async Task<IActionResult> End(Guid chatId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            await _chatService.ExecuteAction(chatId, new EndChatAction(workTokenContext.UserId), ct);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{chatId}/users/{userId}")]
        public async Task<IActionResult> RemoveUser(Guid chatId, Guid userId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            await _chatService.ExecuteAction(chatId, new RemoveUserAction(workTokenContext.UserId, userId), ct);

            return Ok();
        }
    }
}
