using AIChatWebServer.Models.User;

namespace AIChatWebServer.Integrations.Telegram.Models
{
    public sealed record UserWithRegionContext
    (
        User User,
        Region Region
    );
}
