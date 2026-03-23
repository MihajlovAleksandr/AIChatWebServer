namespace AIChatWebServer.Models.Files
{
    public record FileResult
    (
        Stream Stream,
        string ContentType,
        string FileName,
        FileType FileType
    );

}
