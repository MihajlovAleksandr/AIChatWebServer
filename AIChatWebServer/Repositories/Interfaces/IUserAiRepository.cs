using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserAiRepository : ITransactionalScope<IUserAiRepository>
    {
        Task CreateAsync(UserAiModel userAiModel, CancellationToken ct = default);

        Task<UserAiModel?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<UserAiModel?> GetByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default);

        Task<bool> ExistsByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default);

        Task<IReadOnlyList<UserAiModel>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}