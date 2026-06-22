namespace AIChatWebServer.Models.Ranks
{
    public class PointTransaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}