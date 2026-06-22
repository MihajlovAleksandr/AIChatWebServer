using AIChatWebServer.Models.Chats;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record CreateChatRequest
    {
        [JsonPropertyName("chatType")]
        public required ChatType ChatType { get; init; }
        [JsonPropertyName("chatName")]
        public required string ChatName { get; init; }
    }
}
