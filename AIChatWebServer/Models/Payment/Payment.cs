namespace AIChatWebServer.Models.Payment
{
    public sealed record Payment
    {
        public Guid Id { get; init; }
        public string? TransactionId { get; set; }
        public string? StripeChargeId { get; set; }
        public string? StripeInvoiceUrl { get; set; }
        public Guid UserId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = default!;
        public PaymentStatuses Status { get; set; }
        public DateTime CreatedAt { get; init; }
    }
}