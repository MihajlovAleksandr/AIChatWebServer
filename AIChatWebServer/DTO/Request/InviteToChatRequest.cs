using AIChatWebServer.Models.Chats;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public record InviteToChatRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; } = "New Chat";

        [JsonPropertyName("roleOnJoin")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required ChatUserRole RoleOnJoin { get; init; } = ChatUserRole.Member;
    }
}
