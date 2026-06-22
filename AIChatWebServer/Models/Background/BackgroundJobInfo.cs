namespace AIChatWebServer.Models.Background
{
    public class BackgroundJobInfo
    {
        public Guid JobId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string JobType { get; set; }
        public Func<IServiceProvider, CancellationToken, Task> Action { get; set; }
    }
}
