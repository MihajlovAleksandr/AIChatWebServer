using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class PaymentResponseMapper(IResponseMapper<Product, ProductResponse> productMapper) : IResponseMapper<(Payment payment, List<PaymentItem> items), PaymentResponse>
    {
        private readonly IResponseMapper<Product, ProductResponse> _productMapper = productMapper;

        public PaymentResponse ToResponse((Payment payment, List<PaymentItem> items) model)
        {
            return new PaymentResponse(
                model.payment.Id,
                model.payment.StripeInvoiceUrl,
                model.payment.Amount,
                model.payment.Currency,
                model.payment.Status,
                model.payment.CreatedAt,
                model.items.Select(i => new PaymentItemResponse(
                    _productMapper.ToResponse(i.Product),
                    i.Quantity)));
        }
    }
}
