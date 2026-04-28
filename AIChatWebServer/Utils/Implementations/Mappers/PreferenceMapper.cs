using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class PreferenceMapper : IMapper<PreferenceRequest, Preference, PreferenceResponse>
    {
        public Preference ToModel(PreferenceRequest request)
        {
            return new Preference(Guid.NewGuid(), request.MinAge, request.MaxAge, request.Gender);
        }

        public PreferenceResponse ToResponse(Preference model)
        {
            return new PreferenceResponse(model.MinAge, model.MaxAge, model.Gender);
        }
    }
}
