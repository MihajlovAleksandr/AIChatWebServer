using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    public class PaymentInfo
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; } = string.Empty;
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EUR";
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
