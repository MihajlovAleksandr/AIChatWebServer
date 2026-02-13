using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record NotificationSettingsResponse
    (
        [property: JsonPropertyName("emailNotificationsEnabled")] bool EmailNotificationsEnabled
    );
}
