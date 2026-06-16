namespace AIChatWebServer.Services.Interfaces.Utils
{
    public interface IHasher
    {
        string Hash(string data);
        Task<string> HashAsync(
            Stream stream,
            CancellationToken ct = default);
        bool Verify(string data, string hashedData);
    }
}
