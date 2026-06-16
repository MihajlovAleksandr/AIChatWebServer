using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Hubs.Interfaces;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController(
        IConnectionValidator connectionValidator,
        IChatService chatService,
        IResponseMapper<ChatWithUserContext, ChatResponse> chatResponseMapper,
        IChatCreateStrategiesHandlerFactory chatCreateStrategiesHandlerFactory,
        IChatGroupNotifier chatNotifier) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IChatService _chatService = chatService;
        private readonly IChatCreateStrategiesHandlerFactory _chatCreateStrategiesHandlerFactory = chatCreateStrategiesHandlerFactory;
        private readonly IResponseMapper<ChatWithUserContext, ChatResponse> _chatResponseMapper = chatResponseMapper;
        private readonly IChatGroupNotifier _chatNotifier = chatNotifier;

        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(
            Guid chatId, 
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext, 
            CancellationToken ct) 
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId, 
                workTokenContext.UserId, 
                clientContext.Device, ct); 

            Chat chat = await _chatService.GetById(chatId, ct);

            ChatResponse chatResponse = _chatResponseMapper.ToResponse(new ChatWithUserContext(chat, workTokenContext.UserId)); 
            return Ok(chatResponse);
        }

        [Authorize]
        [HttpPut("{chatId}/name")]
        public async Task<IActionResult> UpdateName(
            Guid chatId,
            [FromBody] ChatNameRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _chatService.ExecuteAction(
                chatId,
                new UpdateNameAction(workTokenContext.UserId, request.Name),
                ct);

            await _chatNotifier.ChatNameUpdated(
                chatId,
                workTokenContext.UserId,
                workTokenContext.ConnectionId,
                request.Name,
                ct);

            return Ok();
        }

        [Authorize]
        [HttpPost("{chatId}/end")]
        public async Task<IActionResult> End(
            Guid chatId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _chatService.ExecuteAction(
                chatId,
                new EndChatAction(workTokenContext.UserId),
                ct);

            Chat chat = await _chatService.GetById(chatId, ct);
 
            await _chatNotifier.ChatEnded(
                chat,
                workTokenContext.ConnectionId,
                ct);
            if (chat.EndTime == null)
                throw new ArgumentException();

            return Ok(new EndChatResponse(chat.EndTime.Value));
        }

        [Authorize]
        [HttpDelete("{chatId}/users/{userId?}")]
        public async Task<IActionResult> RemoveUser(
            Guid chatId,
            Guid? userId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Guid removeUserId = userId ?? workTokenContext.UserId;

            await _chatService.ExecuteAction(
                chatId,
                new RemoveUserAction(workTokenContext.UserId, removeUserId),
                ct);

            await _chatNotifier.UserRemoved(
                chatId,
                removeUserId,
                workTokenContext.ConnectionId,
                ct);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateChat(
            [FromBody] CreateChatRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Guid chatId = await _chatCreateStrategiesHandlerFactory
                .Create()
                .CreateAsync(
                    request.ChatType,
                    workTokenContext.UserId,
                    request.ChatName,
                    ct);

            Chat chat = await _chatService.GetById(chatId, ct);

            ChatResponse response = _chatResponseMapper.ToResponse(
                new ChatWithUserContext(chat, workTokenContext.UserId));

            await _chatNotifier.ChatCreated(
                chatId,
                workTokenContext.UserId,
                workTokenContext.ConnectionId,
                ct);

            return Ok(response);
        }
    }
}