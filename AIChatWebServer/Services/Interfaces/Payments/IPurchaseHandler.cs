using AIChatWebServer.Models.Payment;
using AIChatWebServer.Repositories.Interfaces;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPurchaseHandler
    {
        bool CanHandle(Product product, PaymentData data);

        Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            IUnitOfWork uow,
            CancellationToken ct);
    }
}