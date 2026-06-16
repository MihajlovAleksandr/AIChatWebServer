using AIChatWebServer.Models.Exceptions.Implementations.Auth;
using AIChatWebServer.Models.Exceptions.Implementations.Connection;
using AIChatWebServer.Models.Exceptions.Implementations.Context;
using AIChatWebServer.Models.User;
using AIChatWebServer.Repositories.Interfaces;
using AIChatWebServer.Services.Interfaces.Connections;

namespace AIChatWebServer.Services.Implementations.Connections
{
    public sealed class ConnectionValidator(IConnectionService connectionService, IUserRepository userRepository) : IConnectionValidator
    {
        private readonly IConnectionService _connectionService = connectionService;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task ValidateConnectionAsync(
            Guid connectionId,
            Guid userId,
            string? device, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(device))
                throw new DeviceMissingException();

            Models.Connection.ConnectionInfo connectionInfo =
                await _connectionService.GetConnectionInfoAsync(connectionId, cancellationToken)
                ?? throw new ConnectionNotFoundException(connectionId);

            if (connectionInfo.UserId != userId)
            {
                throw new ConnectionAccessDeniedException(
                    connectionId,
                    userId,
                    connectionInfo.UserId);
            }

            if (!string.Equals(connectionInfo.Device, device, StringComparison.Ordinal))
            {
                throw new InvalidConnectionDeviceException(
                    connectionId,
                    device,
                    connectionInfo.Device);
            }
            UserBan? ban = await _userRepository.GetUserBanByIdAsync(userId, cancellationToken);
            if (ban != null && ban.IsActual())
                throw new UserBannedException(ban);
        }

    }
}
