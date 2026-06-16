using System.Text.Json;

namespace AIChatWebServer.Models.Themes
{
    public class Theme
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ThemeType Type { get; set; }
        public JsonDocument ConfigJson { get; set; } = null!;
        public long UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? FileId { get; set; }
    }
}
