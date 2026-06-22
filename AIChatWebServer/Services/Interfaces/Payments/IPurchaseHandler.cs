using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;
using Npgsql;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IPurchaseHandler : ITransactionalScope<IPurchaseHandler>
    {
        bool CanHandle(Product product, PaymentData data);

        Task ApplyAsync(
            Guid userId,
            PaymentItem item,
            PaymentData data,
            CancellationToken ct);
    }
}