using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncChatsResponseMapper(
        ICollectionResponseMapper<ChatWithUserContext, ChatResponse> chatMapper
    ) : IResponseMapper<SyncChats, SyncChatsResponse>
    {
        private readonly ICollectionResponseMapper<ChatWithUserContext, ChatResponse> _chatMapper = chatMapper;

        public SyncChatsResponse ToResponse(SyncChats model)
        {
            return new SyncChatsResponse(
                _chatMapper.ToResponse(model.NewChats),
                _chatMapper.ToResponse(model.UpdatedChats),
                model.DeletedChats
            );
        }
    }
}