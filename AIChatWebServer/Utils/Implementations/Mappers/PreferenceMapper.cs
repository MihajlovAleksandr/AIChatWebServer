using AIChatWebServer.DTO.Request;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class PreferenceMapper : IRequestMapper<PreferenceRequest, Preference>
    {
        public Preference ToModel(PreferenceRequest request)
        {
            return new Preference(Guid.NewGuid(), request.MinAge, request.MaxAge, request.Gender);
        }
    }
}
