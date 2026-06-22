using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Chats;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Services.Interfaces.AI;
using AIChatWebServer.Services.Interfaces.Messages;
using AIChatWebServer.Utils.AI;

namespace AIChatWebServer.Services.Implementations.Messages.Processors
{
    public class AIChatMessageProcessor(IConfiguration configuration, IAIService aIService, IAISettingsService aiSettingsService) : IMessageProcessor
    {
        private readonly IAIService _aIService = aIService;
        private readonly IAISettingsService _aiSettingsService = aiSettingsService;
        private readonly Guid _aIId =  Guid.TryParse(configuration["SystemUsers:AIId"], out Guid id) 
            ? id : throw new ArgumentException("AI Id is not valid.");
        
        public async Task<ProcessorResult?> ProcessAsync(Message message, Chat chat, CancellationToken ct)
        {
            AISettingsModel model = await _aiSettingsService.GetByChatId(chat.Id, ct);
            string prompt = model.CustomPrompt ?? AIPrompts.AIDefault.ToString();
            string response = await _aIService.SendMessageAsync(chat.Id, model.Model, prompt, message.Text, ct);
            return new ProcessorResult(response, _aIId);
        }
    }
}
