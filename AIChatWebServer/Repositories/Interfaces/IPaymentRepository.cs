using AIChatWebServer.Models.Payment;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IPaymentRepository : ITransactionRepository<IPaymentRepository>
    {
        Task CreateAsync(Payment payment, CancellationToken ct = default);

        Task ConfirmAsync(Guid paymentId, string transactionId, CancellationToken ct = default);

        Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<List<Payment>> GetHistoryAsync(Guid userId, CancellationToken ct = default);

        Task<bool> ExistsByTransactionId(string transactionId, CancellationToken ct = default);
    }
}