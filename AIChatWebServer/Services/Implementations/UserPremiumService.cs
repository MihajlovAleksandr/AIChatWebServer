using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public class UserPremiumService(IUserPremiumRepository userPremiumRepository) : IUserPremiumService
    {
        private readonly IUserPremiumRepository _userPremiumRepository = userPremiumRepository;
 
        public async Task<UserPremium?> GetActiveAsync(Guid userId, CancellationToken ct)
        {
            return await _userPremiumRepository.GetActiveAsync(userId, ct);
        }

        public async Task<UserPremium?> GetLastAsync(Guid userId, CancellationToken ct)
        {
            return await _userPremiumRepository.GetLastAsync(userId, ct);
        }
        public async Task CreateAsync(
                    Guid userId,
                    Guid paymentItemId,
                    DateTime startAt,
                    DateTime endAt,
                    bool isAutoRenew = false,
                    string? subscriptionId = null,
                    CancellationToken ct = default)
        {

            await _userPremiumRepository.CreateAsync(userId, paymentItemId, startAt, endAt, isAutoRenew, subscriptionId, ct);
        }

        public async Task<List<UserPremium>> GetHistoryAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            return await _userPremiumRepository.GetHistoryAsync(userId, ct);
        }

        public async Task<Guid?> GetUserIdBySubscriptionIdAsync(
            string subscriptionId,
            CancellationToken ct = default)
        {
            return await _userPremiumRepository.GetUserIdBySubscriptionIdAsync(subscriptionId, ct);
        }

        public Task<UserPremium?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _userPremiumRepository.GetByIdAsync(id, ct);
        }

        public Task<UserPremium?> GetAutoRenewAsync(Guid userId, CancellationToken ct = default)
        {
            return _userPremiumRepository.GetAutoRenewAsync(userId, ct);
        }

        public Task CancelAutoRenew(string subscriptionId, CancellationToken ct = default)
        {
            return _userPremiumRepository.CancelAutoRenew(subscriptionId, ct);
        }
    }
}
