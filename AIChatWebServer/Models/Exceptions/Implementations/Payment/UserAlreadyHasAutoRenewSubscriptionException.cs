using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Payment
{
    public class UserAlreadyHasAutoRenewSubscriptionException(Guid userId)
        : ApiExceptionBase(
            409,
            PaymentErrors.UserAlreadyHasAutoRenewSubscription,
            $"User {userId} already has an active auto-renew subscription")
    {
    }
}
