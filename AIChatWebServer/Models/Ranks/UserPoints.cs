namespace AIChatWebServer.Models.Ranks
{
    public class UserPoints
    {
        public Guid UserId { get; set; }
        public int TotalPoints { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}