namespace AIChatWebServer.Models.Payment
{
    public record PaymentCredentials(PaymentType Type, object Data)
    {
        public PaymentType Type { get; init; } = Type;
        public object Data { get; init; } = Data;
    }
}
