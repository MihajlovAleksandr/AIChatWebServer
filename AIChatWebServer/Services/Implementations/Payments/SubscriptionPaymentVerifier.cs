using AIChatWebServer.Models.Exceptions.Implementations.Payment;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.Payments;
using AIChatWebServer.Services.Interfaces.Users;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class SubscriptionPaymentVerifier(IUserPremiumService userPremiumService) : IPaymentVerifier
    {
        private readonly IUserPremiumService _userPremiumService = userPremiumService;

        public bool CanHandle(PaymentType type)
        {
            return type == PaymentType.Subscription;
        }

        public async Task Verify(List<(Product product, int quantity)> paymentItems, PaymentType paymentType, Guid userId, CancellationToken ct)
        {
            if (paymentItems.Count != 1)
                throw new InvalidSubscriptionPaymentItemsException(paymentItems.Count);

            UserPremium? userPremium =
                await _userPremiumService.GetActiveAsync(userId, ct);

            if (userPremium != null)
                throw new UserAlreadyHasActiveSubscriptionException(userId);
        }
    }
}
