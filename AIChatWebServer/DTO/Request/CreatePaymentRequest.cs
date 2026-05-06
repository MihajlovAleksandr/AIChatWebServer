using AIChatWebServer.Models.Payment;
using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record CreatePaymentRequest
    {
        [JsonPropertyName("items")]
        public required List<PaymentItemRequest> Items { get; init; }
        [JsonPropertyName("type")]
        public required PaymentType Type { get; init; }
    }
}
