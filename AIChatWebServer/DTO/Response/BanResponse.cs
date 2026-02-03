using AIChatWebServer.Models.User;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record BanResponse
    (
        [property: JsonPropertyName("reason")] string Reason,
        [property: JsonPropertyName("reasonCategory")] BanReason ReasonCategory,
        [property: JsonPropertyName("bannedUntil")] DateTime BannedUntil
    );
}