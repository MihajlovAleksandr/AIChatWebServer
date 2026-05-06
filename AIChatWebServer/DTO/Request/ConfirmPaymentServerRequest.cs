using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record ConfirmPaymentServerRequest
    {
        [JsonPropertyName("transactionId")]
        public required string TransactionId { get; init; }
    }
}
