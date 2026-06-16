using AIChatWebServer.Models.Exceptions.Implementations.AI;
using AIChatWebServer.Models.Exceptions.Implementations.Payment;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Payments;
using System.Text.Json;

namespace AIChatWebServer.Services.Implementations.Payments
{
    public class SingleItemPaymentVerifier(IUserAiService userAiService) : IPaymentVerifier
    {
        private readonly IUserAiService _userAiService = userAiService;

        public bool CanHandle(PaymentType type)
        {
            return type == PaymentType.SingleItem;
        }

        public async Task Verify(List<(Product product, int quantity)> paymentItems, PaymentType paymentType, Guid userId, CancellationToken ct)
        {
            foreach (var (product, quantity) in paymentItems)
            {
                if (quantity != 1)
                    throw new InvalidSingleItemPaymentItemsException(quantity);

                var attrs = JsonSerializer.Deserialize<ModelAtributes>(product.AttributesJson)
                    ?? throw new ArgumentException("Invalid json attributes");

                if (await _userAiService.ExistsByUserIdAndModelAsync(userId, attrs.Model, ct))
                    throw new UserAlreadyHasAIModelException(userId, attrs.Model);
            }
        }
    }
}
