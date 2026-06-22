using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UserInfoResponseMapper(IResponseMapper<UserData, UserDataResponse> mapper) : IResponseMapper<UserInfo, UserInfoResponse>
    {
        private readonly IResponseMapper<UserData, UserDataResponse> _mapper = mapper;
        public UserInfoResponse ToResponse(UserInfo model)
        {
            return new UserInfoResponse(_mapper.ToResponse(model.UserData), model.LastOnline, model.RegionCode);
        }
    }
}
