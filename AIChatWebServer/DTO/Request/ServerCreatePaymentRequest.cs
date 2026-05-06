using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record ServerCreatePaymentRequest
    {
        [JsonPropertyName("userId")]
        public required Guid UserId { get; init; }

        [JsonPropertyName("items")]
        public required List<PaymentItemRequest> Items { get; init; }
    }
}
