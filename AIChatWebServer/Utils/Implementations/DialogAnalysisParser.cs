using AIChatWebServer.Models.Messages;
using AIChatWebServer.Utils.Interfaces;
using System.Text.RegularExpressions;

namespace AIChatWebServer.Utils.Implementations
{
    public class DialogAnalysisParser : IDialogAnalysisParser
    {        
        private readonly Dictionary<Guid, int> _userNum = new Dictionary<Guid, int>();
        private readonly Dictionary<int, Guid> _numUser = new Dictionary<int, Guid>();

        public string CreateText(IEnumerable<Message> messages)
        {
            string message = "";

            foreach (Message msg in messages)
            {
                if (_userNum.TryGetValue(msg.UserId, out int value))
                {
                    message += $"@{value}: {msg.Text}\n";
                }
                else
                {
                    int userId = _userNum.Count + 1;
                    _userNum.Add(msg.UserId, userId);
                    _numUser.Add(userId, msg.UserId);
                    message += $"@{userId}: {msg.Text}\n";
                }
            }

            return message;
        }

        public string ReplaceWithGuids(string aiResponse)
        {
            return Regex.Replace(aiResponse, @"@(\d+)", match =>
            {
                int userId = int.Parse(match.Groups[1].Value);
                return _numUser.ContainsKey(userId) ? _numUser[userId].ToString() : match.Value;
            });
        }
    }
}