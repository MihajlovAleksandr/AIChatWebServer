using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Exceptions.Implementations.Chat;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class ChatResponseMapper : IResponseMapper<ChatWithUserContext, ChatResponse>
    {
        public ChatResponse ToResponse(ChatWithUserContext model)
        {
            if(!model.Chat.UsersWithData.TryGetValue(model.UserId, out ChatUserData? chatUserData))
            {
                throw new UserNotInChatException(model.Chat.Id, model.UserId);
            }

            return new ChatResponse(model.Chat.Id, model.Chat.Type, model.Chat.EndTime, model.Chat.UsersWithData.Keys, chatUserData.Name);
        }
    }
}
