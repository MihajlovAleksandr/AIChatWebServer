using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public record LeaderboardItemResponse
    (
        [property: JsonPropertyName("userInfo")] UserInfoResponse UserInfo,
        [property: JsonPropertyName("points")] int PointsCount
    );
}
