using AIChatWebServer.Integrations.AI.Configuration;
using AIChatWebServer.Integrations.AI.DTO;
using AIChatWebServer.Integrations.AI.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AIChatWebServer.Integrations.AI.Implementations
{
    public class DeepSeekController(
        HttpClient httpClient,
        IOptions<DeepSeekSettings> settings,
        ILogger<DeepSeekController> logger) : IAIController
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<DeepSeekController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly DeepSeekSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

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
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                var totalTokens = root
                    .GetProperty("usage")
                    .GetProperty("total_tokens")
                    .GetInt32();

                logger.LogInformation("Parsed AI response. Tokens: {Tokens}", totalTokens);

                return answer == null
                    ? null
                    : new AIMessageResponse(answer, totalTokens);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parsing AI response");
                throw;
            }
        }

        public async Task<AIMessageResponse?> SendMessageAsync(
            IEnumerable<AIMessageRequest> aiMessages,
            string model)
        {
            var request = new
            {
                model,
                messages = aiMessages,
                stream = false,
                max_tokens = _settings.MaxTokenCount
            };

            var json = JsonSerializer.Serialize(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);

            try
            {
                _logger.LogInformation("Sending request to DeepSeek");

                var response = await _httpClient.SendAsync(httpRequest);

                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Received response: {Response}", content);

                response.EnsureSuccessStatusCode();

                return Parse(content, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeepSeek request failed");
                throw;
            }
        }
    }
}