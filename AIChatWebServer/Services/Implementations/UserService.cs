using AIChatWebServer.Models.Exceptions.Implementations;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces;

namespace AIChatWebServer.Services.Implementations
{
    public sealed class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<bool> IsPremium(Guid userId, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new UserNotFoundException(userId);

            foreach (var premium in user.Premium)
            {
                if (premium.EndTime > DateTime.UtcNow)
                    return true;
            }
            return false;
        }

        public async Task UpdatePreference(Guid userId, Preference preference, CancellationToken cancellationToken)
        {
            await _userRepository.SavePreferenceAsync(userId, preference, cancellationToken);
        }

        public async Task UpdateUserData(Guid userId, UserData userData, CancellationToken cancellationToken)
        {
            await _userRepository.SaveUserDataAsync(userId, userData, cancellationToken);
        }
    }
}
