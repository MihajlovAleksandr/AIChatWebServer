namespace AIChatWebServer.Models.Ranks
{
    public class RankLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinPoints { get; set; }
        public int? MaxPoints { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}