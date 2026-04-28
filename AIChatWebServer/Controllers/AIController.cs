using AIChatWebServer.Integrations.AI;
using AIChatWebServer.Models.AI;
using AIChatWebServer.Models.Messages;
using AIChatWebServer.Services.Interfaces.AI;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AIChatWebServer.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("translate")]
        public async Task<IActionResult> Translate(
            [FromBody] TranslateRequest request,
            CancellationToken ct)
        {
            var result = await _aiService.TranslateAsync(
                request.ChatId,
                request.Model,
                request.LangCode,
                request.Style,
                request.Message);

            return Ok(new { result });
        }

        [HttpPost("compress-message")]
        public async Task<IActionResult> CompressMessage(
            [FromBody] CompressMessageRequest request,
            CancellationToken ct)
        {
            var result = await _aiService.CopmressMessageAsync(
                request.ChatId,
                request.Model,
                request.Message);

            return Ok(new { result });
        }

        [HttpPost("compress-dialog")]
        public async Task<IActionResult> CompressDialog(
            [FromBody] CompressDialogRequest request,
            CancellationToken ct)
        {
            var result = await _aiService.CompressDialogAsync(
                request.ChatId,
                request.Model,
                request.Messages);

            return Ok(new { result });
        }
    }

    public class TranslateRequest
    {
        public Guid ChatId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIModel Model { get; set; }
        public string LangCode { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TranslateStyle Style { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CompressMessageRequest
    {
        public Guid ChatId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIModel Model { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CompressDialogRequest
    {
        public Guid ChatId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AIModel Model { get; set; }
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}