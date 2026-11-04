using DocuMind.Application.Interfaces.AI;
using DocuMind.Infrastructure.AI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocuMind.Infrastructure.AI
{
    public class GeminiChatService : IGenerateAIResponseService
    {
        private readonly HttpClient _httpClient;
        private readonly ChatOptions _options;

        public GeminiChatService(HttpClient httpClient, IOptions<ChatOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var apiKey = _options.ApiKey;
            var baseUrl = _options.BaseUrl;
            var model = _options.Model;

            ArgumentException.ThrowIfNullOrEmpty(apiKey, "Gemini API key is not configured");
            ArgumentException.ThrowIfNullOrEmpty(baseUrl, "Gemini API base URL is not configured");
            ArgumentException.ThrowIfNullOrEmpty(model, "Gemini API model is not configured");

            var url = $"{baseUrl}/models/{model}:generateContent";

            var requestBody = new
            {
                contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = _options.Temperature,
                        maxOutputTokens = _options.MaxOutputTokens
                    }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("x-goog-api-key", apiKey);

            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Gemini API error ({(int)response.StatusCode}): " +
                    responseContent);
            }

            using var document = JsonDocument.Parse(responseContent);

            var candidates = document.RootElement.GetProperty("candidates");

            var parts = candidates[0].GetProperty("content").GetProperty("parts");

            var answer = string.Join(
                "\n",
                parts.EnumerateArray()
                    .Where(p => p.TryGetProperty("text", out _))
                    .Select(p => p.GetProperty("text").GetString())
            );

            return answer;

        }
    }
}
