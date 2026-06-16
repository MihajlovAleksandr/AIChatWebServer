using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.User;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserPremiumRepository
        : ITransactionalScope<IUserPremiumRepository>
    {
        Task CreateAsync(
                    Guid userId,
                    Guid paymentItemId,
                    DateTime startAt,
                    DateTime endAt,
                    bool isAutoRenew = false,
                    string? subscriptionId = null,
                    CancellationToken ct = default);

        Task<UserPremium?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task<UserPremium?> GetActiveAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<UserPremium?> GetLastAsync(Guid userId,
            CancellationToken ct = default);

        Task<List<UserPremium>> GetHistoryAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<Guid?> GetUserIdBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default);

        Task<UserPremium?> GetFirstBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default);

        Task<UserPremium?> GetAutoRenewAsync(Guid userId, 
            CancellationToken ct = default);

        Task CancelAutoRenew(string subscriptionId,
            CancellationToken ct = default);
    }
}