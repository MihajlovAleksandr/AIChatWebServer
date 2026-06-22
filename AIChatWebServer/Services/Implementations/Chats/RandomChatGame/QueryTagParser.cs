using AIChatWebServer.Models.Chats.RandomChat;
using AIChatWebServer.Services.Interfaces.Chats.RandomChatGame;

namespace AIChatWebServer.Services.Implementations.Chats.RandomChatGame
{
    public class QueryTagParser : IQueryTagParser
    {
        public QueryTags Parse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return QueryTags.Casual;

            var tags = QueryTags.None;

            var parts = response
                .ToLower()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                tags |= part switch
                {
                    "casual" => QueryTags.Casual,
                    "smalltalk" => QueryTags.SmallTalk,
                    "personal" => QueryTags.Personal,
                    "emotional" => QueryTags.Emotional,
                    "knowledge" => QueryTags.Knowledge,
                    "technical" => QueryTags.Technical,
                    "complex" => QueryTags.Complex,
                    _ => QueryTags.None
                };
            }

            return tags == QueryTags.None ? QueryTags.Casual : tags;
        }
    }
}