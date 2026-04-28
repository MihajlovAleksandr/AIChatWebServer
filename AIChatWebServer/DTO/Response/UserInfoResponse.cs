using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record UserInfoResponse
    (
        [property: JsonPropertyName("userData")] UserDataResponse UserData,
        [property: JsonPropertyName("lastOnline")] DateTime? LastOnline,
        [property: JsonPropertyName("region")] string region
    );
}
