using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Predicates
{
    public class UserMatchPredicateFactory : IUserMatchPredicateFactory
    {
        public IUserMatchPredicate Create(string name)
        {
            return name switch
            {
                "AllMatch" => new AllMatchPredicate(),
                "UserPreferenceMatch" => new UserPreferenceMatchPredicate(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
