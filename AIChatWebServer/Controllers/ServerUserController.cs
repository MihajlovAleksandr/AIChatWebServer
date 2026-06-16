using AIChatWebServer.Integrations.Telegram.DTO.Request;
using AIChatWebServer.Integrations.Telegram.DTO.Response;
using AIChatWebServer.Integrations.Telegram.Models;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Context.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;
using AIChatWebServer.Services.Interfaces.Users;
using AIChatWebServer.Utils.Interfaces.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("server/users")]
    public class ServerUserController(
        IServerValidator serverValidator, 
        IUserService userService, 
        IUserDeletionService deletionService, 
        ICollectionResponseMapper<User, TelegramContextResponse> contextMapper,
        IResponseMapper<UserWithRegionContext, TelegramUserResponse> userMapper) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IServerValidator _serverValidator = serverValidator;
        private readonly IUserDeletionService _deletionService = deletionService;
        private readonly ICollectionResponseMapper<User, TelegramContextResponse> _contextMapper = contextMapper;
        private readonly IResponseMapper<UserWithRegionContext, TelegramUserResponse> _userMapper = userMapper;



        [Authorize]
        [HttpGet("auth/{provider}")]
        public async Task<IActionResult> GetByAuthProviderCode(
            [FromServices] IServerTokenContext tokenContext,
            [FromRoute] string provider,
            CancellationToken ct)
        {
            await _serverValidator.Validate(tokenContext.UserId, tokenContext.Server, ct);
            var users = await _userService.GetByAuthProviderCode(provider, ct);
            return Ok(_contextMapper.ToResponse(users));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            [FromServices] IServerTokenContext tokenContext,
            [FromRoute] Guid id, 
            CancellationToken ct)
        {
            await _serverValidator.Validate(tokenContext.UserId, tokenContext.Server, ct);

            User user = await _userService.GetByIdAsync(id, ct);
            return Ok(_userMapper.ToResponse(new UserWithRegionContext(user, await _userService.GetRegionByCode(user.RegionCode, ct))));
        }

        [Authorize]
        [HttpPut("language")]
        public async Task<IActionResult> UpdateUserLanguage(
            [FromServices] IServerTokenContext tokenContext,
            [FromBody] UpdateUserLanguageServerRequest request,
            CancellationToken ct)
        {
            await _serverValidator.Validate(tokenContext.UserId, tokenContext.Server, ct);
            foreach(LanguageContext context in request.Context)
                await _userService.UpsertUserLanguageAsync(request.UserId, context, request.Language, ct);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId}/auth/{provider}")]
        public async Task<IActionResult> DeleteAuthProvider(
            [FromServices] IServerTokenContext tokenContext,
            [FromRoute] Guid userId,
            [FromRoute] string provider,
            CancellationToken ct)
        {
            await _serverValidator.Validate(tokenContext.UserId, tokenContext.Server, ct);
            await _userService.DeleteAuthIdentityAsync(userId, provider, ct);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(
            [FromServices] IServerTokenContext tokenContext,
            [FromRoute] Guid userId,
            CancellationToken ct)
        {
            await _serverValidator.Validate(tokenContext.UserId, tokenContext.Server, ct);
            await _deletionService.DeleteAsync(userId, ct);
            return Ok();
        }
    }
}
