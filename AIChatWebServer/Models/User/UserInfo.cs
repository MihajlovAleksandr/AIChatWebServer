namespace AIChatWebServer.Models.User
{
    public sealed record UserInfo
    (
        UserData UserData,
        DateTime? LastOnline,
        string RegionCode
    );
}
