namespace AIChatWebServer.Services.Interfaces
{
    public interface IHasher
    {
        string Hash(string data);
        bool Verify(string data, string hashedData);
    }
}
