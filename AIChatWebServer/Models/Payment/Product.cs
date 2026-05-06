namespace AIChatWebServer.Models.Payment
{
    public sealed class Product
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public PaymentType Type { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = null!;

        public string StripePriceId { get; set; } = null!;

        public string AttributesJson { get; set; } = "{}";

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}