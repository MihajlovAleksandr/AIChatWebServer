namespace AIChatWebServer.Models.Payment
{
    public sealed record PaymentItem
    {
        public Guid Id { get; init; }
        public Guid PaymentId { get; init; }

        public Product Product { get; init; } = default!;
        public int Quantity { get; init; }
        public decimal Price { get; init; }
    }
}