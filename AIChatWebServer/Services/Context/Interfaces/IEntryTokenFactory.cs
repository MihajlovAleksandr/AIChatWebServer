namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IEntryTokenFactory
    {
        string Create(Guid userId, string code);
    }
}
