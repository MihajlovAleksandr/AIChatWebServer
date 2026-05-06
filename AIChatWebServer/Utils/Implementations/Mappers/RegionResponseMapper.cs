using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class RegionResponseMapper : IResponseMapper<Region, RegionResponse>
    {
        public RegionResponse ToResponse(Region model)
        {
            return new RegionResponse(model.Code, model.Name, model.Currency);
        }
    }
}
