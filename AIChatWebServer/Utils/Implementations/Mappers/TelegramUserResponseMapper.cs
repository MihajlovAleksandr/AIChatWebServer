using AIChatWebServer.DTO.Response;
using AIChatWebServer.Integrations.Telegram.DTO.Response;
using AIChatWebServer.Integrations.Telegram.Models;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class TelegramUserResponseMapper(IResponseMapper<Region, RegionResponse> regionMapper,
        ICollectionResponseMapper<UserPremium, UserPremiumResponse> premiumMapper) : IResponseMapper<UserWithRegionContext, TelegramUserResponse>
    {
        private readonly IResponseMapper<Region, RegionResponse> _regionMapper = regionMapper;
        private readonly ICollectionResponseMapper<UserPremium, UserPremiumResponse> _premiumMapper = premiumMapper;

        public TelegramUserResponse ToResponse(UserWithRegionContext model)
        {
            AuthIdentity authIdentity = model.User.GetAuthIdentity("TELEGRAM")
                ?? throw new ArgumentException($"User {model.User.Id} is not telegram user");
            if (!long.TryParse(authIdentity.Identifier, out long tgUserId))
                throw new ArgumentException($"TELEGRAM auth identity identifier is not tgUserId. User {model.User.Id}");

            return new TelegramUserResponse(model.User.Id,
                tgUserId, model.User.Email,
                model.User.Language[LanguageContext.Telegram],
                _regionMapper.ToResponse(model.Region),
                _premiumMapper.ToResponse(model.User.Premium));
        }
    }
}
