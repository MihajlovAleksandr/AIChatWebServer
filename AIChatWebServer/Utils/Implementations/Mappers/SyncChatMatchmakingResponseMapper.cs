using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncChatMatchmakingResponseMapper
        : IResponseMapper<SyncChatMatchmaking, SyncChatMatchmakingResponse>
    {
        public SyncChatMatchmakingResponse ToResponse(SyncChatMatchmaking model)
        {
            return new SyncChatMatchmakingResponse(
                model.IsSearching
            );
        }
    }
}