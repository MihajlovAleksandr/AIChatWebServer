using AIChatWebServer.Contracts.UnitOfWork.Interfaces;
using AIChatWebServer.Models.AI;

namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IUserAiService : ITransactionalScope<IUserAiService>
    {
        Task<UserAiModel> CreateAsync(Guid userId, AIModel model, Guid paymentId, CancellationToken ct = default);
        Task<UserAiModel> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<UserAiModel> GetByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default);
        Task<bool> ExistsByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default);
        Task<IReadOnlyList<UserAiModel>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
