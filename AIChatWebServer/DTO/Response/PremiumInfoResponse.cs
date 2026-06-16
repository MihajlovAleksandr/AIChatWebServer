using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record PremiumInfoResponse
    (
        [property: JsonPropertyName("startTime")] DateTime? StartTime,
        [property: JsonPropertyName("endTime")] DateTime? EndTime,
        [property: JsonPropertyName("isAutoRenew")] bool IsAutoRenew
    );
}
