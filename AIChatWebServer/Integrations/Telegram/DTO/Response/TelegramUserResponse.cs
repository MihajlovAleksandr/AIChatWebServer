using AIChatWebServer.DTO.Response;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Integrations.Telegram.DTO.Response
{
    public sealed record TelegramUserResponse
    (
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonProperty("tgUserId")] long TgUserId,
        [property: JsonProperty("email")] string Email,
        [property: JsonProperty("language")] string Language,
        [property: JsonProperty("region")] RegionResponse Region,
        [property: JsonProperty("userPremium")]IReadOnlyCollection<UserPremiumResponse> UserPremium
    );
}
