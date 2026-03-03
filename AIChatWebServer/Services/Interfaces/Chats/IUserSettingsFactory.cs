namespace AIChatWebServer.Services.Interfaces.Chats
{
    public interface IUserSettingsFactory
    {
        IChatRulesValidator Create();
    }
}
