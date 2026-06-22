using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UserPremiumMapper : IMapper<UserPremiumRequest, UserPremium, UserPremiumResponse>
    {
        public UserPremium ToModel(UserPremiumRequest request)
        {
            return new UserPremium
            {
                Id = request.Id,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
        }

        public UserPremiumResponse ToResponse(UserPremium model)
        {
            return new UserPremiumResponse(model.Id, model.StartTime, model.EndTime, model.IsAutoRenew);
        }
    }
}
