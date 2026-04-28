using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Models.Sync;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public sealed class SyncMessagesResponseMapper(
        ICollectionResponseMapper<MessageContext, MessageResponse> messageMapper
    ) : IResponseMapper<SyncMessages, SyncMessagesResponse>
    {
        private readonly ICollectionResponseMapper<MessageContext, MessageResponse> _messageMapper = messageMapper;

        public SyncMessagesResponse ToResponse(SyncMessages model)
        {
            return new SyncMessagesResponse(
                _messageMapper.ToResponse(model.NewMessages),
                _messageMapper.ToResponse(model.UpdatedMessages),
                model.DeletedMessages
            );
        }
    }
}