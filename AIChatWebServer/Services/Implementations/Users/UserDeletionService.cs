using AIChatWebServer.Models.Exceptions.Implementations.User;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Users;
using Microsoft.Extensions.Options;

namespace AIChatWebServer.Services.Implementations.Users
{
    public sealed class UserDeletionService(
        IUserDeletionRepository repository,
        IOptions<SystemUsersOptions> systemUsersOptions)
        : IUserDeletionService
    {
        private readonly IUserDeletionRepository _repository = repository;
        private readonly SystemUsersOptions _systemUsers = systemUsersOptions.Value;

        public async Task DeleteAsync(
            Guid userId,
            CancellationToken ct = default)
        {
            ValidateUserId(userId);

            await EnsureUserCanBeDeleted(userId, ct);

            bool deleted = await _repository.DeleteUser(userId, _systemUsers.DeletedUserId, ct);

            if (!deleted)
                throw new UserNotFoundException(userId);
        }

        private void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is empty", nameof(userId));

            if (userId == _systemUsers.DeletedUserId)
                throw new CannotDeleteSystemUserException(userId);

            if (userId == _systemUsers.AIId)
                throw new CannotDeleteSystemUserException(userId);
        }

        private async Task EnsureUserCanBeDeleted(Guid userId, CancellationToken ct)
        {
            var user = await _repository.GetUserMeta(userId, ct);

            if (user is null)
                throw new UserNotFoundException(userId);

            if (user.DeletedStatus)
                throw new UserAlreadyDeletedException(userId);
        }
    }
}