using AIChatWebServer.Models.Messages;

namespace AIChatWebServer.Utils.Interfaces
{
    public interface IDialogAnalysisParser
    {
        string CreateText(IEnumerable<Message> messages);
        public string ReplaceWithGuids(string aiResponse);
    }
}
