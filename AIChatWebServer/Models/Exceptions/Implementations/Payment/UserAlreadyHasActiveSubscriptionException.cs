using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Payment
{
    public class UserAlreadyHasActiveSubscriptionException(Guid userId)
    : ApiExceptionBase(
        409,
        PaymentErrors.UserAlreadyHasActiveSubscription,
        $"User {userId} already has an active subscription")
    {
    }
}
