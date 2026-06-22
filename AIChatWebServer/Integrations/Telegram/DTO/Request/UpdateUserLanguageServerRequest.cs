using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Integrations.Telegram.DTO.Request
{
    public sealed record UpdateUserLanguageServerRequest
    {
        [JsonPropertyName("language")]
        public required string Language { get; init; }
        [JsonPropertyName("context")]
        public required IReadOnlyList<LanguageContext> Context { get; init; }
        [JsonPropertyName("userId")]
        public required Guid UserId { get; init; }
    }
}
