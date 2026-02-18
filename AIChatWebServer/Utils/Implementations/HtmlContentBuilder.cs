using AIChatWebServer.Utils.Interfaces;

namespace AIChatWebServer.Utils.Implementations
{
    public sealed class HtmlContentBuilder : IHtmlContentBuilder
    {
        private readonly IStringChanger _stringChanger;

        private const string ImageToken = "[IMAGE]";

        public HtmlContentBuilder(IStringChanger stringChanger)
        {
            _stringChanger = stringChanger
                ?? throw new ArgumentNullException(nameof(stringChanger));
        }

        public HtmlContentResult BuildHtml(
            string text,
            string[] imagePaths)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            imagePaths ??= Array.Empty<string>();

            var imageMap = new Dictionary<string, string>();

            var resultText = text;

            var maxCount =
                Math.Min(
                    CountTokens(resultText),
                    imagePaths.Length);

            for (int i = 0; i < maxCount; i++)
            {
                var path = imagePaths[i];

                if (!File.Exists(path))
                    continue;

                var cid = Guid.NewGuid().ToString("N");

                imageMap[path] = cid;

                resultText = _stringChanger.Replace(
                    resultText,
                    ImageToken,
                    cid,
                    1);
            }

            return new HtmlContentResult(
                WrapHtml(resultText),
                imageMap);
        }

        private int CountTokens(string text)
        {
            var count = 0;
            var index = 0;

            while (true)
            {
                index = text.IndexOf(
                    ImageToken,
                    index,
                    StringComparison.Ordinal);

                if (index < 0)
                    break;

                count++;
                index += ImageToken.Length;
            }

            return count;
        }

        private string WrapHtml(string body)
        {
            return $"""
                   <!DOCTYPE html>
                   <html>
                     <head>
                       <meta charset="utf-8"/>
                     </head>
                     <body>
                       {body}
                     </body>
                   </html>
                   """;
        }
    }
}
