using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Payment;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class PaymentInfoResponseMapper : IResponseMapper<Payment, PaymentInfoResponse>
    {
        public PaymentInfoResponse ToResponse(Payment model)
        {
            return new PaymentInfoResponse(model.Id, model.Amount, model.Currency, model.Status, model.CreatedAt);
        }
    }
}
