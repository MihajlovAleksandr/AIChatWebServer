namespace AIChatWebServer.Models.Payment
{
    public abstract record PaymentData;
    public sealed record SubscriptionPaymentData(string SubscriptionId, bool WillAutoCharge, DateTime CreatedAt, DateTime CurrentPeriodStart, DateTime CurrentPeriodEnd) : PaymentData;
    public sealed record OneTimePaymentData : PaymentData;
}
