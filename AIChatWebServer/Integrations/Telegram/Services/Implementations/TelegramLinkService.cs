using AIChatWebServer.Integrations.Telegram.Models;
using AIChatWebServer.Integrations.Telegram.Services.Interfaces;
using AIChatWebServer.Models.Links;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.Users;
using AIChatWebServer.Services.Interfaces.Utils;

namespace AIChatWebServer.Integrations.Telegram.Services.Implementations
{
    public class TelegramLinkService(ILinkService linkService, IUserService userService) : ITelegramLinkService
    {
        private readonly ILinkService _linkService = linkService;
        private readonly IUserService _userService = userService;

        public async Task<string> GenerateAsync(Guid userId, CancellationToken ct)
        {
            return await _linkService.CreateAsync(LinkType.TelegramBind, "{}", userId, DateTime.UtcNow.AddMinutes(10), 1, ct);
        }

        public async Task<UserWithRegionContext> BindTelegramAsync(long tgId, string token, string language, CancellationToken ct)
        {
            Link link = await _linkService.ExecuteAsync(token, ct);
            await _userService.AddAuthIdentity(link.CreatedBy, tgId.ToString(), "TELEGRAM", null, ct);
            await _userService.UpsertUserLanguageAsync(link.CreatedBy, LanguageContext.Telegram, language, ct);
            User user = await _userService.GetByIdAsync(link.CreatedBy, ct);
            return new UserWithRegionContext(user, await _userService.GetRegionByCode(user.RegionCode));
        }
    }
}
