using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Exceptions.Implementations.AI;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.AI;
using Npgsql;

namespace AIChatWebServer.Services.Implementations.AI
{
    public class UserAiService : IUserAiService
    {
        private readonly IUserAiRepository _userAiRepository;
        private readonly NpgsqlConnection? _conn;
        private readonly NpgsqlTransaction? _tx;


        public UserAiService(IUserAiRepository userAiRepository)
        {
            _userAiRepository = userAiRepository;
        }

        private UserAiService(IUserAiRepository userAiRepository, NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
            _userAiRepository = userAiRepository.WithTransaction(_conn, _tx);
        }

        public IUserAiService WithTransaction(NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            return new UserAiService(_userAiRepository, conn, tx);
        }

        public async Task<UserAiModel> CreateAsync(Guid userId, AIModel model, Guid paymentItemId, CancellationToken ct = default)
        {
            if (await _userAiRepository.ExistsByUserIdAndModelAsync(userId, model, ct))
                throw new UserAlreadyHasAIModelException(userId, model);

            UserAiModel userAiModel = UserAiModel.Create(userId, model, paymentItemId);
            await _userAiRepository.CreateAsync(userAiModel, ct);
            return userAiModel;
        }

        public Task<bool> ExistsByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default)
        {
            return _userAiRepository.ExistsByUserIdAndModelAsync(userId, model, ct);
        }

        public async Task<IReadOnlyList<UserAiModel>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _userAiRepository.GetAllByUserIdAsync(userId, ct);
        }

        public async Task<UserAiModel> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _userAiRepository.GetByIdAsync(id, ct)
                ?? throw new UserAiModelNotFoundException(id);
        }

        public async Task<UserAiModel> GetByUserIdAndModelAsync(Guid userId, AIModel model, CancellationToken ct = default)
        {
            return await _userAiRepository.GetByUserIdAndModelAsync(userId, model, ct)
                ?? throw new AIModelNotAvailableForUserException(userId, model);
        }
    }
}
