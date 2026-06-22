namespace AIChatWebServer.Services.Interfaces.AI
{
    public interface IAIMessageDispatcherFactory
    {
        IAIMessageDispatcher Create(Guid chatId);
    }
}