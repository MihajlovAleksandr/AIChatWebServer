using AIChatWebServer.DTO.Request;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class PreferenceMapper : IRequestMapper<PreferenceRequest, Preference>
    {
        public Preference ToModel(PreferenceRequest request)
        {
            return new Preference(Guid.NewGuid(), request.MinAge, request.MaxAge, request.Gender);
        }
    }
}
