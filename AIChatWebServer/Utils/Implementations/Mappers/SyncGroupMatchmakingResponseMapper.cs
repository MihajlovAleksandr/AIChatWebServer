using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncGroupMatchmakingResponseMapper
        : IResponseMapper<SyncGroupMatchmaking, SyncGroupMatchmakingResponse>
    {
        public SyncGroupMatchmakingResponse ToResponse(SyncGroupMatchmaking model)
        {
            return new SyncGroupMatchmakingResponse(
                model.IsSearching,
                model.ChatId
            );
        }
    }
}