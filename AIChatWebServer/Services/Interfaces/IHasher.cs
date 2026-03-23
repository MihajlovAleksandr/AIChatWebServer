namespace AIChatWebServer.Services.Interfaces
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
