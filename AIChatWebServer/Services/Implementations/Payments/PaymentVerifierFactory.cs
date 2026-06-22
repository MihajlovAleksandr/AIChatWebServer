using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.Payments;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class PaymentVerifierFactory(IEnumerable<IPaymentVerifier> verifiers) : IPaymentVerifierFactory
    {
        private readonly IEnumerable<IPaymentVerifier> _verifiers = verifiers;

        public IPaymentVerifier Create(PaymentType type)
        {
            foreach(var verifier in _verifiers)
            {
                if (verifier.CanHandle(type))
                    return verifier;
            }
            throw new ArgumentException($"No payment verifier for type {type}");
        }
    }
}
