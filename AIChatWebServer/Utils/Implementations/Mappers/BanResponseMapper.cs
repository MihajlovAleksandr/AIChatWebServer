using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class BanResponseMapper : IResponseMapper<UserBan, BanResponse>
    {
        public BanResponse ToResponse(UserBan ban)
        {
            ArgumentNullException.ThrowIfNull(ban);

            return new BanResponse
            (   ban.Reason,
                ban.ReasonCategory,
                ban.BannedUntil
            );
        }

    }
}
