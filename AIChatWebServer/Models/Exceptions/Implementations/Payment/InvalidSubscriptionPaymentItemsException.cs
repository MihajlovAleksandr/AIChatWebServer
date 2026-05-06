using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Payment
{
    public class InvalidSubscriptionPaymentItemsException(int itemsCount)
        : ApiExceptionBase(
            400,
            PaymentErrors.InvalidSubscriptionPaymentItems,
            $"Subscription payment must contain exactly one item, but got {itemsCount}")
    {
    }
}
