namespace AIChatWebServer.Models.Notification
{
    public class NotificationPrompt
    {
        private readonly string _prompt;
        private NotificationPrompt(string prompt)
        {
            _prompt = prompt;
        }

        public static readonly NotificationPrompt
            NewChat = new NotificationPrompt("new_chat");

        public static readonly NotificationPrompt
            EndChat = new NotificationPrompt("end_chat");

        public static readonly NotificationPrompt
            AddUser = new NotificationPrompt("add_user");

        public static readonly NotificationPrompt
            NewMessage = new NotificationPrompt("new_message");

        public override string ToString()
        {
            return _prompt;
        }
    }
}
