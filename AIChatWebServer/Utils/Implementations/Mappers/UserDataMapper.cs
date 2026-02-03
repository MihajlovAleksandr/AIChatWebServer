using AIChatWebServer.DTO.Request;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UserDataMapper : IRequestMapper<UserDataRequest, UserData>
    {
        public UserData ToModel(UserDataRequest request)
        {
            return new UserData(Guid.NewGuid(), request.Gender, request.Name, request.Age );
        }
    }
}
