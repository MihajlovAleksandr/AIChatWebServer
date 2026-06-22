using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record NotificationSettingsResponse
    (
        [property: JsonPropertyName("emailNotificationsEnabled")] bool EmailNotificationsEnabled
    );
}
