using AIChatWebServer.Models.User;
using AIChatWebServer.Utils.Errors;

namespace AIChatWebServer.Models.Exceptions.Implementations.User
{
    public class PremiumRequiredException(Guid userId, PremiumFeature feature)
        : ApiExceptionBase(
            403,
            UserErrors.PremiumRequired,
            $"User '{userId}' attempted to access feature '{feature}', which requires a premium subscription")
    {
        public PremiumFeature Feature { get; init; } = feature;
    }
}