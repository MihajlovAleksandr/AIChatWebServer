using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class MessageResponseMapper(ICollectionResponseMapper<MessageReply, MessageReplyResponse> repliesMapper) : IResponseMapper<MessageContext, MessageResponse>
    {
        private readonly ICollectionResponseMapper<MessageReply, MessageReplyResponse> _repliesMapper = repliesMapper;
        public MessageResponse ToResponse(MessageContext model)
        {
            IReadOnlyDictionary<Guid, MessageStatus> statuses;
            if (model.CanSeeOtherUsersStatuses && model.Message.UserId == model.UserId)
            {
                statuses = model.Message.Statuses;
            }
            else
            {
                if(!model.Message.Statuses.TryGetValue(model.UserId, out MessageStatus status))
                {
                    status = MessageStatus.None;
                }
                statuses = new Dictionary<Guid, MessageStatus>()
                {
                    { model.UserId, status }
                };
            }
            return new MessageResponse(
                model.Message.Id, 
                model.Message.ChatId, 
                model.Message.UserId, 
                model.Message.Text, 
                model.Message.Time,
                model.Message.LastUpdate, 
                _repliesMapper.ToResponse(model.Message.Replies),
                statuses, 
                model.Message.Files.Select(f => f.Id).ToList());
        }
    }
}
