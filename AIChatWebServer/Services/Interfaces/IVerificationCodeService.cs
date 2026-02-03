namespace AIChatWebServer.Services.Interfaces
{
    public interface IVerificationCodeService
    {
        Task<string> GenerateAsync(
            Guid userId,
            string type,
            CancellationToken cancellationToken = default);

        Task VerifyAsync(
            Guid userId,
            string type,
            string code,
            CancellationToken cancellationToken = default);
    }
}
