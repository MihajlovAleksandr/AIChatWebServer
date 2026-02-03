namespace AIChatWebServer.Services.Tokens.Interfaces
{
    public interface IWorkTokenFactory
    {
        string Create(Guid userId, Guid connectionId);
    }
}
