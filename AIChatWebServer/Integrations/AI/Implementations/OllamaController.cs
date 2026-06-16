using AIChatWebServer.Integrations.AI.Configuration;
using AIChatWebServer.Integrations.AI.DTO;
using AIChatWebServer.Integrations.AI.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AIChatWebServer.Integrations.AI.Implementations
{
    public class OllamaController(
        HttpClient httpClient,
        IOptions<OllamaSettings> settings,
        ILogger<OllamaController> logger) : IAIController
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<OllamaController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly OllamaSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        private static AIMessageResponse? Parse(string? aiResponse, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(aiResponse))
            {
                logger.LogWarning("AI response is null or empty.");
                return null;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(aiResponse);
                var root = document.RootElement;

                var answer = root
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                var promptTokens = root.GetProperty("prompt_eval_count").GetInt32();
                var completionTokens = root.GetProperty("eval_count").GetInt32();
                var totalTokens = promptTokens + completionTokens;

                logger.LogInformation(
                    "Parsed Ollama response. Prompt: {PromptTokens}, Completion: {CompletionTokens}, Total: {TotalTokens}",
                    promptTokens,
                    completionTokens,
                    totalTokens
                );

                return answer == null
                    ? null
                    : new AIMessageResponse(answer, totalTokens);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parsing Ollama response");
                throw;
            }
        }

        public async Task<AIMessageResponse?> SendMessageAsync(
            IEnumerable<AIMessageRequest> aiMessages,
            string model,
            bool isSystemPrompt = false)
        {
            if (aiMessages == null || !aiMessages.Any())
            {
                _logger.LogWarning("Empty message list.");
                return null;
            }

            var request = new
            {
                model = model,
                messages = aiMessages,
                stream = false,
                options = new
                {
                    thinking = false
                }
            };

            var json = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            try
            {
                _logger.LogInformation("Sending request to Ollama with model {Model}", model);

                var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                return Parse(content, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ollama request failed");
                throw;
            }
        }
    }
}