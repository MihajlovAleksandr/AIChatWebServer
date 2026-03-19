namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IUserMatchPredicateFactory
    {
        IUserMatchPredicate Create(string name);
    }
}
