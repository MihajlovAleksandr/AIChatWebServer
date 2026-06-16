using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Exceptions.Implementations.Themes;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Models.Themes;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Users;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/themes")]
    public class ThemeController(
        IConnectionValidator connectionValidator,
        IThemeService themeService,
        ICollectionResponseMapper<Theme, ThemeResponse> themeCollectionResponseMapper,
        IResponseMapper<UploadSession, UploadSessionResponse> sessionMapper,
        IRequestMapper<UploadSessionFileRequest, UploadSessionFile> sessionFileMapper,
        IResponseMapper<Theme, ThemeResponse> themeResponseMapper) : ControllerBase
    {
        private readonly IConnectionValidator _connectionValidator = connectionValidator;
        private readonly IThemeService _themeService = themeService;
        private readonly ICollectionResponseMapper<Theme, ThemeResponse> _themeCollectionResponseMapper = themeCollectionResponseMapper;
        private readonly IResponseMapper<UploadSession, UploadSessionResponse> _sessionMapper = sessionMapper;
        private readonly IResponseMapper<Theme, ThemeResponse> _themeResponseMapper = themeResponseMapper;
        private readonly IRequestMapper<UploadSessionFileRequest, UploadSessionFile> _sessionFileMapper = sessionFileMapper;

        [Authorize]
        [HttpGet("{themeId}")]
        public async Task<IActionResult> GetThemeById(
            Guid themeId,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Theme theme = await _themeService.GetThemeByIdAsync(themeId, ct);
            ThemeResponse response = _themeResponseMapper.ToResponse(theme);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetThemeByName(
            string name,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Theme theme = await _themeService.GetThemeByNameAsync(workTokenContext.UserId, name, ct);
            ThemeResponse response = _themeResponseMapper.ToResponse(theme);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetThemesByUserId(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            IReadOnlyList<Theme> themes = await _themeService.GetThemesByUserIdAsync(workTokenContext.UserId, ct);
            return Ok(_themeCollectionResponseMapper.ToResponse(themes));
        }

        [Authorize]
        [HttpGet("system")]
        public async Task<IActionResult> GetThemesByType(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            IReadOnlyList<Theme> themes = await _themeService.GetThemesByTypeAsync(ThemeType.System, ct);
            return Ok(_themeCollectionResponseMapper.ToResponse(themes));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTheme(
            [FromBody] CreateThemeRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Guid themeId = await _themeService.CreateThemeAsync(
                workTokenContext.UserId,
                request.Name,
                ThemeType.Custom,
                request.Content,
                ct);

            Theme theme = await _themeService.GetThemeByIdAsync(themeId, ct);
            ThemeResponse response = _themeResponseMapper.ToResponse(theme);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheme(
            [FromRoute] Guid id,
            [FromBody] UpdateThemeRequest request,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Theme theme = await _themeService.GetThemeByIdAsync(id, ct);
            if (theme.UserId != workTokenContext.UserId)
                throw new ThemeAccessDeniedException(theme.Id, workTokenContext.UserId);
            theme.Name = request.Name;
            theme.ConfigJson = request.Content;

            Theme updatedTheme = await _themeService.UpdateThemeAsync(theme, ct);
            ThemeResponse response = _themeResponseMapper.ToResponse(updatedTheme);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheme(
            Guid id,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _themeService.DeleteThemeAsync(id, ct);

            return Ok();
        }

        [Authorize]
        [HttpGet("selected")]
        public async Task<IActionResult> GetSelectedThemeByConnectionId(
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            Theme theme = await _themeService.GetSelectedThemeByConnectionIdAsync(workTokenContext.ConnectionId, ct);

            ThemeResponse response = _themeResponseMapper.ToResponse(theme);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("selected/{id}")]
        public async Task<IActionResult> SetSelectedTheme(
            Guid id,
            [FromServices] IWorkTokenContext workTokenContext,
            [FromServices] IClientContext clientContext,
            CancellationToken ct)
        {
            await _connectionValidator.ValidateConnectionAsync(
                workTokenContext.ConnectionId,
                workTokenContext.UserId,
                clientContext.Device,
                ct);

            await _themeService.GetThemeByIdAsync(id, ct);

            await _themeService.SetSelectedThemeAsync(workTokenContext.ConnectionId, id, ct);
            return Ok();
        }
    }
}