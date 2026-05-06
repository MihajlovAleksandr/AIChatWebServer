using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Payment
{
    public class UserAutoRenewSubscriptionNotFoundException(Guid userId)
        : ApiExceptionBase(
            404,
            PaymentErrors.UserAutoRenewSubscriptionNotFound,
            $"User {userId} does not have an active auto-renew subscription")
    {
    }
}
