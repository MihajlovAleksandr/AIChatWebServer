namespace AIChatWebServer.Services.Interfaces.Users
{
    public interface IUserDeletionService
    {
        Task DeleteAsync(
            Guid userId,
            CancellationToken ct = default);
    }
}