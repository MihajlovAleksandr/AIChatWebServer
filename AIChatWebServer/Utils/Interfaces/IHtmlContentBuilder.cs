namespace AIChatWebServer.Utils.Interfaces
{
    public interface IHtmlContentBuilder
    {
        HtmlContentResult BuildHtml(
            string text,
            string[] imagePaths);
    }

    public class HtmlContentResult(
        string html,
        IReadOnlyDictionary<string, string> imageMap)
    {
        public string Html { get; } = html;

        public IReadOnlyDictionary<string, string> ImageMap { get; } = imageMap;
    }

}
