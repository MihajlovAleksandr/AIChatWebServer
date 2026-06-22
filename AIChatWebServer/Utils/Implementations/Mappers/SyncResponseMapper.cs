using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncResponseMapper(
        IResponseMapper<SyncMatchmaking, SyncMatchmakingResponse> matchmakingMapper,
        IResponseMapper<SyncMessages, SyncMessagesResponse> messagesMapper,
        IResponseMapper<SyncChats, SyncChatsResponse> chatsMapper
    ) : IResponseMapper<SyncModel, SyncResponse>
    {
        private readonly IResponseMapper<SyncMatchmaking, SyncMatchmakingResponse> _matchmakingMapper = matchmakingMapper;
        private readonly IResponseMapper<SyncMessages, SyncMessagesResponse> _messagesMapper = messagesMapper;
        private readonly IResponseMapper<SyncChats, SyncChatsResponse> _chatsMapper = chatsMapper;

        public SyncResponse ToResponse(SyncModel model)
        {
            return new SyncResponse(
                _matchmakingMapper.ToResponse(model.SyncMatchmaking),
                _messagesMapper.ToResponse(model.SyncMessages),
                _chatsMapper.ToResponse(model.SyncChats)
            );
        }
    }
}