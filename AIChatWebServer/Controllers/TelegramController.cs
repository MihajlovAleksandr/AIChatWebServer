using AIChatWebServer.Integrations.Telegram.DTO.Request;
using AIChatWebServer.Integrations.Telegram.DTO.Response;
using AIChatWebServer.Integrations.Telegram.Models;
using AIChatWebServer.Integrations.Telegram.Services.Interfaces;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/integration/telegram")]
    public class TelegramController(IConnectionValidator connectionValidator, IServerValidator serverValidator, ITelegramLinkService telegramLinkService, IResponseMapper<UserWithRegionContext, TelegramUserResponse> userMapper) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IServerValidator _serverValidator = serverValidator;
        private readonly ITelegramLinkService _telegramLinkService = telegramLinkService;
        private readonly IResponseMapper<UserWithRegionContext, TelegramUserResponse> _userMapper = userMapper;

        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateLink(
            [FromServices] IWorkTokenContext workToken,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(workToken.ConnectionId, workToken.UserId, clientContext.Device, ct);
            return Ok(await _telegramLinkService.GenerateAsync(workToken.UserId, ct));
        }

        [Authorize]
        [HttpPost("bind")]
        public async Task<IActionResult> BindTelegram(
            [FromServices] IServerTokenContext serverToken,
            [FromBody] BindTelegramServerRequest request,
            CancellationToken ct)
        {
            await _serverValidator.Validate(serverToken.UserId, serverToken.Server, ct);
            return Ok(_userMapper.ToResponse(await _telegramLinkService.BindTelegramAsync(request.TgUserId, request.Token, request.Language, ct)));
        }
    }
}
