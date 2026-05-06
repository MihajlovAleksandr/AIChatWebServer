namespace AIChatWebServer.Models.Payment
{
    public abstract record PaymentData;
    public sealed record SubscriptionPaymentData(string SubscriptionId, bool IsAutoRenew) : PaymentData;
    public sealed record OneTimePaymentData : PaymentData;
}
