namespace AIChatWebServer.Services.Interfaces
{
    public interface IUserDeletionService
    {
        Task DeleteAsync(
            Guid userId,
            CancellationToken ct = default);
    }
}