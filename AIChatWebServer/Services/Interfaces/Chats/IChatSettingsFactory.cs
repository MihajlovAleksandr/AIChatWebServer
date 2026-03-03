namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IChatSettingsFactory
    {
        IChatRulesValidator Create();
    }
}
