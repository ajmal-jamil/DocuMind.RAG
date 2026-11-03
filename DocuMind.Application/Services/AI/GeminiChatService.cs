using DocuMind.Application.Interfaces.AI;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DocuMind.Application.Services.AI
{
    public class GeminiChatService : IGenerateAIResponseService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["AI:Chat:ApiKey"];

            ArgumentException.ThrowIfNullOrEmpty(apiKey, "Gemini API key is not configured");
            
            var baseUrl = _configuration["AI:Chat:BaseUrl"];
            var model = _configuration["AI:Chat:Model"];

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
                        maxOutputTokens = 256
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
