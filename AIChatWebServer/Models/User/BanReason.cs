using System.Text.Json.Serialization;

namespace AIChatWebServer.Models.User
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum BanReason
    {
        Undefined,
        CommunityRulesViolation,
        SpamOrAdvertising,
        OffensiveLanguage,
        SecurityViolation,
        UnauthorizedToolsUsage,
        HarassmentOrDiscrimination,
        MultipleUserReports,
        AdministrativeDecision,
    }
}
