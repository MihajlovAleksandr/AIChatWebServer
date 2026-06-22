using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class AISettingsResponseMapper : IResponseMapper<AISettingsWithModels, AISettingsResponse>
    {
        public AISettingsResponse ToResponse(AISettingsWithModels model)
        {
            return new AISettingsResponse(model.Settings.Model, model.Settings.CustomPrompt, model.Models);
        }
    }
}
