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
    public class ChatLinkController(IConnectionValidator connectionValidator,
        IChatLinkService chatLinkService,
        IResponseMapper<ChatWithUserContext, ChatResponse> chatResponseMapper) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IChatLinkService _chatLinkService = chatLinkService;
        private readonly IResponseMapper<ChatWithUserContext, ChatResponse> _chatResponseMapper = chatResponseMapper;
        [Authorize]
        [HttpPost("{chatId}/invite")]
        public async Task<IActionResult> InviteUser(Guid chatId,
            [FromBody] InviteToChatRequest inviteToChatRequest,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            string token = await _chatLinkService.CreateInviteLink(chatId, 
                new InviteUserToChatAction(workTokenContext.UserId,
                    inviteToChatRequest.RoleOnJoin,
                    inviteToChatRequest.Name), ct);

            return Ok(token);
        }

        [Authorize]
        [HttpPost("join/{token}")]
        public async Task<IActionResult> JoinChat(
            string token,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            Chat chat = await _chatLinkService.EnterChatViaInviteLink(token, workTokenContext.UserId, ct);

            return Ok(_chatResponseMapper.ToResponse(new ChatWithUserContext(chat, workTokenContext.UserId)));
        }
    }
}
