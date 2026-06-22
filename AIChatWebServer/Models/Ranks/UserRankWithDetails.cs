namespace AIChatWebServer.Models.Ranks
{
    public class UserRankWithDetails
    {
        public Guid UserId { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; } = string.Empty;
        public int MinPoints { get; set; }
        public int? MaxPoints { get; set; }
        public int Priority { get; set; }
        public DateTime ChangedAt { get; set; }
        public int PointsAtMoment { get; set; }
    }
}