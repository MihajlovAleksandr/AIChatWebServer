using AIChatWebServer.DTO.Response;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class ConnectionResponseMapper : IResponseMapper<Models.Connection.ConnectionInfo, ConnectionResponse>
    {
        public ConnectionResponse ToResponse(Models.Connection.ConnectionInfo model)
        {
            return new ConnectionResponse(model.Id, model.UserId, model.Device, model.LastOnline);
        }
    }
}
