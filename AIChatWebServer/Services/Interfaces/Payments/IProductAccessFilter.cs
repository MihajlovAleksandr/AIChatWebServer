using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Services.Interfaces.Payments
{
    public interface IProductAccessFilter : ITransactionalScope<IProductAccessFilter>
    {
        Task<bool> ShouldInclude(Product product, Guid userId, CancellationToken ct = default);
    }
}
