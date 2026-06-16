using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Request
{
    public sealed record ConfirmPaymentServerRequest
    {
        [JsonPropertyName("transactionId")]
        public required string TransactionId { get; init; }
        [JsonPropertyName("stripeInvoiceUrl")]
        public string? StripeInvoiceUrl { get; init; } = null;
        [JsonPropertyName("stripeChargeId")]
        public string? StripeChargeId { get; init; } = null;
    }
}
