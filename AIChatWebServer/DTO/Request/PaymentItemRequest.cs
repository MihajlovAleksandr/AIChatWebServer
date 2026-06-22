using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record PaymentItemRequest
    {
        [JsonPropertyName("productId")]
        public required Guid ProductId { get; init; }

        [JsonPropertyName("quantity")]
        public required int Quantity { get; init; }
    }
}
