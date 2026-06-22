using AIChatWebServer.Models.Chats.RandomChat;

namespace AIChatWebServer.Services.Interfaces.Chats.RandomChatGame
{
    public interface IQueryTagParser
    {
        QueryTags Parse(string response);
    }
}
