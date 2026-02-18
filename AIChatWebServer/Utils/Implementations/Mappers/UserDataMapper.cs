using AIChatWebServer.DTO.Request;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class UserDataMapper : IRequestMapper<UserDataRequest, UserData>
    {
        public UserData ToModel(UserDataRequest request)
        {
            return new UserData(Guid.NewGuid(), request.Gender, request.Name, request.Age );
        }
    }
}
