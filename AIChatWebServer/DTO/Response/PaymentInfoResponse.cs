using AIChatWebServer.Models.Payment;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record PaymentInfoResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("status")] PaymentStatuses Status,
        [property: JsonPropertyName("createdAt")] DateTime CreatedAt
    );
}
