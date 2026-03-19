using AIChatWebServer.Models.User;

namespace AIChatWebServer.Services.Interfaces.Chats.Matchmaking
{
    public interface IUserMatchPredicate
    {
        string Predicate { get; }
        bool TryMatch(User main, User other);
    }
}
