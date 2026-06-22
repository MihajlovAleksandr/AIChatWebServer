using AIChatWebServer.Models.User;

namespace AIChatWebServer.Repositories.Interfaces
{
    public interface IUserDeletionRepository
    {
        Task<bool> DeleteUser(
            Guid userId,
            Guid deletedUserId,
            CancellationToken cancellationToken = default);
        
        Task<UserMeta?> GetUserMeta(Guid userId, CancellationToken ct = default);
    }
}