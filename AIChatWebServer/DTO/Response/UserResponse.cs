using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record UserResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("region")] string Region,
        [property: JsonPropertyName("isPremium")] bool IsPremium,
        [property: JsonPropertyName("userData")] UserDataResponse UserData,
        [property: JsonPropertyName("preference")] PreferenceResponse Preference,
        [property: JsonPropertyName("language")] string Language
    );
      
}
