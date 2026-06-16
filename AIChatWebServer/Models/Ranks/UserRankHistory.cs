namespace AIChatWebServer.Models.Ranks
{
    public class UserRankHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int RankId { get; set; }
        public DateTime ChangedAt { get; set; }
        public int PointsAtMoment { get; set; }
    }
}