namespace AIChatWebServer.Models.Files
{
    public enum UploadSessionStatus
    {
        Created = 0,
        InProgress = 1,
        Completed = 2,
        Canceled = 3,
        Expired = 4
    }
}