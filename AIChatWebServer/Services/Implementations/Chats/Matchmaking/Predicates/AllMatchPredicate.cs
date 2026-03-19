using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Predicates
{
    public class AllMatchPredicate : IUserMatchPredicate
    {
        public string Predicate => "AllMatch";

        public bool TryMatch(User first, User second)
        {
            return true;
        }
    }
}
