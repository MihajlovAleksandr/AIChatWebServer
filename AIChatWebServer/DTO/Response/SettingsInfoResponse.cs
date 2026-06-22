using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record SettingsInfoResponse
    (
        [property: JsonPropertyName("user")] UserResponse User,
        [property: JsonPropertyName("connections")] IReadOnlyCollection<ConnectionResponse> Connections,
        [property: JsonPropertyName("notificationSettings")] NotificationSettingsResponse NotificationSettings
    );
}
