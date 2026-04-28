using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class UserDataMapper : IMapper<UserDataRequest, UserData, UserDataResponse>
    {
        public UserData ToModel(UserDataRequest request)
        {
            return new UserData(Guid.NewGuid(), request.Gender, request.Name, request.Age );
        }

        public UserDataResponse ToResponse(UserData model)
        {
            return new UserDataResponse(model.Name, model.Age, model.Gender);
        }
    }
}
