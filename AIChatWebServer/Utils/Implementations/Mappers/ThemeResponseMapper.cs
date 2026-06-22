using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Themes;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class ThemeResponseMapper : IResponseMapper<Theme, ThemeResponse>
    {
        public ThemeResponse ToResponse(Theme model)
        {
            return new ThemeResponse(model.Id, model.Name, model.Type, model.ConfigJson, model.Type == ThemeType.System ? null : model.UsageCount);
        }
    }
}
