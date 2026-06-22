using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record NotificationSettingsRequest
    {
        [JsonPropertyName("emailNotificationsEnabled")]
        public required bool EmailNotificationsEnabled { get; init; }
    }

}
