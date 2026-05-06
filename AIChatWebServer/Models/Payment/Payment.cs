namespace AIChatWebServer.Models.Payment
{
    public sealed record Payment
    {
        public Guid Id { get; init; }
        public string? TransactionId { get; set; }

        public Guid UserId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; init; }
    }
}