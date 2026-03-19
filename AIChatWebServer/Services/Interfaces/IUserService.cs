using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces
{
    public interface IUserService
    {
        Task UpdateUserData(Guid userId, UserData userData, CancellationToken cancellationToken);
        Task UpdatePreference(Guid userId, Preference preference, CancellationToken cancellationToken);
        Task<bool> IsPremium(Guid userId, CancellationToken cancellationToken);
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
