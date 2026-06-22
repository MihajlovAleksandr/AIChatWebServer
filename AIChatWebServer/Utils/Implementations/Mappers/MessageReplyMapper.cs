using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class MessageReplyMapper : IMapper<MessageReplyRequest, MessageReply, MessageReplyResponse>
    {
        public MessageReply ToModel(MessageReplyRequest request)
        {
            return new MessageReply(request.ReplyMessageId, request.StartIndexQuote, request.EndIndexQuote);
        }

        public MessageReplyResponse ToResponse(MessageReply model)
        {
            return new MessageReplyResponse(model.ReplyMessageId, model.StartIndexQuote, model.EndIndexQuote);
        }
    }
}
