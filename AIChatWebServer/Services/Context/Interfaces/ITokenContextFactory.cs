namespace AIChatWebServer.Services.Context.Interfaces
{
    public interface ITokenContextFactory
    {
        ITokenContext Create(IUserContextAccessor accessor);
    }
}
