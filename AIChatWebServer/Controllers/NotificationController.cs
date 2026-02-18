using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Notification;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public sealed class NotificationController(IConnectionValidator connectionValidator, INotificationService notificationService,
        IMapper<NotificationSettingsRequest, NotificationSettings, NotificationSettingsResponse> notificationMapper) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IMapper<NotificationSettingsRequest, NotificationSettings, NotificationSettingsResponse> _notificationMapper = notificationMapper;

        [Authorize]
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateNotificationSettings(
            [FromServices] IWorkTokenContext tokenContext, 
            [FromServices] IClientContext clientContext, 
            [FromBody] NotificationSettingsRequest notificationSettingsRequest,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, ct);

            NotificationSettings notificationSettings = _notificationMapper.ToModel(notificationSettingsRequest);

            await _notificationService.UpdateSettingsAsync(tokenContext.UserId, notificationSettings, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet("settings")]
        public async Task<IActionResult> NotificationSettings(
            [FromServices] IWorkTokenContext tokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(tokenContext.ConnectionId, tokenContext.UserId, clientContext.Device, ct);

            NotificationSettings notificationSettings = await _notificationService.GetSettingsAsync(tokenContext.UserId, ct);
      
            return Ok(_notificationMapper.ToResponse(notificationSettings));
        }

    }
}
