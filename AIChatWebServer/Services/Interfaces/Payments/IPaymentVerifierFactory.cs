using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPaymentVerifierFactory
    {
        IPaymentVerifier Create(PaymentType type);
    }
}
