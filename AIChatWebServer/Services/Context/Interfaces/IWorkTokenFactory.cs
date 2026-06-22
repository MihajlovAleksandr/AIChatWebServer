namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface IWorkTokenFactory
    {
        string Create(Guid userId, Guid connectionId);
    }
}
