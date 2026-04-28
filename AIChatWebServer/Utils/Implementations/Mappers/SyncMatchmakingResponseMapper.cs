using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncMatchmakingResponseMapper(
        IResponseMapper<SyncChatMatchmaking, SyncChatMatchmakingResponse> chatMapper,
        IResponseMapper<SyncGroupMatchmaking, SyncGroupMatchmakingResponse> groupMapper
    ) : IResponseMapper<SyncMatchmaking, SyncMatchmakingResponse>
    {
        private readonly IResponseMapper<SyncChatMatchmaking, SyncChatMatchmakingResponse> _chatMapper = chatMapper;
        private readonly IResponseMapper<SyncGroupMatchmaking, SyncGroupMatchmakingResponse> _groupMapper = groupMapper;

        public SyncMatchmakingResponse ToResponse(SyncMatchmaking model)
        {
            return new SyncMatchmakingResponse(
                _chatMapper.ToResponse(model.SyncChatMatchmaking),
                _groupMapper.ToResponse(model.SyncGroupMatchmaking)
            );
        }
    }
}