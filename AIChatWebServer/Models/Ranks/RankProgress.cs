namespace AIChatWebServer.Models.Ranks
{
    public class RankProgress
    {
        public RankLevel CurrentRank { get; set; } = null!;
        public RankLevel? NextRank { get; set; }
        public int CurrentPoints { get; set; }
        public int PointsToNextRank { get; set; }
        public double ProgressPercentage { get; set; }
    }
}