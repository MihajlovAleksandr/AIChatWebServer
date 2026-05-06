using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record PaymentRequest
    {
        [JsonPropertyName("transactionId")]
        public required string TransactionId { get; init; }
        [JsonPropertyName("userId")]
        public required Guid UserId { get; init; }
        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }
        [JsonPropertyName("currency")]
        public required string Currency { get; init; }
        [JsonPropertyName("status")]
        public required string Status { get; init; }
        [JsonPropertyName("timestamp")]
        public required DateTime Timestamp { get; init; }
    }
}
