using AIChatWebServer.Models.Payment;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record ServerCreatePaymentRequest
    {
        [JsonPropertyName("userId")]
        public required Guid UserId { get; init; }

        [JsonPropertyName("type")]
        public PaymentType Type { get; init; } = PaymentType.Subscription;

        [JsonPropertyName("items")]
        public required List<PaymentItemRequest> Items { get; init; }
    }
}
