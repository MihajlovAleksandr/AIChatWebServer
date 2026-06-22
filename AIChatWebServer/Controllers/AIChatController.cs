using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Chats;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/chat/{chatId}/ai")]
    public class AIChatController(IResponseMapper<AISettingsWithModels, AISettingsResponse> responseMapper, IAISettingsService settingsService, IConnectionValidator connectionValidator, IChatService chatService) : ControllerBase
    {
        private readonly IResponseMapper<AISettingsWithModels, AISettingsResponse> _responseMapper = responseMapper;
        private readonly IChatService _chatService = chatService;
        private readonly IAISettingsService _settingsService = settingsService;
        private readonly IConnectionValidator _connectionValidator = connectionValidator;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSettings(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid chatId,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            Chat chat = await _chatService.GetById(chatId, ct);
            if(chat.Type != ChatType.AI)
                throw new ChatTypeNotSupportedException(chat.Type);

            if (!chat.UsersWithData.ContainsKey(workTokenContext.UserId))
                throw new UserDoesNotBelongToChatException(chatId, workTokenContext.UserId);

            AISettingsModel settings = await _settingsService.GetByChatId(chatId, ct);
            IReadOnlyCollection<AIModel> models = await _settingsService.GetAvibleModels(workTokenContext.UserId, ct);

            return Ok(_responseMapper.ToResponse(new AISettingsWithModels(settings, models)));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateSettings(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            [FromRoute] Guid chatId,
            [FromBody] AISettingsRequest request,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workTokenContext.ConnectionId, workTokenContext.UserId, clientContext.Device, ct);

            Chat chat = await _chatService.GetById(chatId, ct);
            if (chat.Type != ChatType.AI)
                throw new ChatTypeNotSupportedException(chat.Type);

            if (!chat.UsersWithData.ContainsKey(workTokenContext.UserId))
                throw new UserDoesNotBelongToChatException(chatId, workTokenContext.UserId);

            await _settingsService.CreateOrUpdate(workTokenContext.UserId, chatId, request.AIModel, request.Prompt, ct);
            return Ok();
        }
    }
}