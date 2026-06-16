using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.Payment
{
    public class InvalidSingleItemPaymentItemsException(int itemsCount)
        : ApiExceptionBase(
            400,
            PaymentErrors.InvalidPaymentItems,
            $"SingleItem payment must contain exactly one item, but got {itemsCount}");
}
