using AIChatWebServer.Models.User;
using AIChatWebServer.Services.Interfaces.Chats.Matchmaking;

namespace AIChatWebServer.Services.Implementations.Chats.Matchmaking.Predicates
{
    public class UserPreferenceMatchPredicate : IUserMatchPredicate
    {
        public string Predicate => "UserPreferenceMatch";

        public bool TryMatch(User main, User other)
        {
            if(main.UserData == null)
            {
                ArgumentNullException.ThrowIfNull(main.UserData);
            }

            if(other.Preference == null)
            {
                ArgumentNullException.ThrowIfNull(other.Preference);
            }

            return main.UserData.IsFits(other.Preference);
        }
    }
}
