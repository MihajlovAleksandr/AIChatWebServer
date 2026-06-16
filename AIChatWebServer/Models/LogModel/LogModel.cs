namespace AIChatWebServer.Models.LogModel
{
    public sealed class LogModel
    {
        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public string Level { get; }
        public string Message { get; }
        public string? Source { get; }

        public LogModel(
            Guid id,
            DateTime timestamp,
            string level,
            string message,
            string? source)
        {
            Id = id;
            Timestamp = timestamp;
            Level = level;
            Message = message;
            Source = source;
        }
    }
}