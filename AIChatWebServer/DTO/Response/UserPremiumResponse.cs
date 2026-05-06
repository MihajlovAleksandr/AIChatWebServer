using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record UserPremiumResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("startTime")] DateTime StartTime,
        [property: JsonPropertyName("endTime")] DateTime EndTime
    );
}
