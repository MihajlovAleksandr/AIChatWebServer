namespace AIChatWebServer.Models.User
{
    public sealed class UserPremium()
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
