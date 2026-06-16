using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IProductAccessRule : ITransactionalScope<IProductAccessRule>
    {
        bool CanHandle(PaymentType type);
        Task<bool> Handle(Product product, Guid userId, CancellationToken ct = default);
    }
}
