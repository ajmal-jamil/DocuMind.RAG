using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Infrastructure.AI.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocuMind.Infrastructure.AI
{
    public class GeminiEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly EmbeddingOptions _options;

        public GeminiEmbeddingService(HttpClient httpClient, IOptions<EmbeddingOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public Task<float[]> GenerateDocumentEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
        {
            var formattedText = $"title: none | text: {text}";

            return GenerateEmbeddingAsync(
                formattedText,
                cancellationToken);
        }

        public Task<float[]> GenerateQueryEmbeddingAsync(
        string question,
        CancellationToken cancellationToken = default)
        {
            var formattedQuestion =
                $"task: question answering | query: {question}";

            return GenerateEmbeddingAsync(
                formattedQuestion,
                cancellationToken);
        }

        private async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty.");

            var apiKey = _options.ApiKey;
            var baseUrl = _options.BaseUrl;
            var model = _options.Model;

            ArgumentException.ThrowIfNullOrEmpty(apiKey, "Gemini API key is not configured");
            ArgumentException.ThrowIfNullOrEmpty(baseUrl, "Gemini API base URL is not configured");
            ArgumentException.ThrowIfNullOrEmpty(model, "Gemini API model is not configured");

            int dimensions = _options.Dimensions;

            ArgumentException.ThrowIfNullOrEmpty(baseUrl, "Gemini API base URL is not configured");
            ArgumentException.ThrowIfNullOrEmpty(model, "Gemini API model is not configured");
                        
            var url = $"{baseUrl}/models/{model}:embedContent";

            var requestBody = new
            {
                content = new
                {
                    parts = new[]
                    {
                        new { text }
                    }
                },
                output_dimensionality = dimensions
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post, url);

            request.Headers.Add("x-goog-api-key", apiKey);
            request.Content = JsonContent.Create(requestBody);

            using var response = await _httpClient.SendAsync(
                request, cancellationToken);

            var responseContent =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Gemini embedding API error: {responseContent}");
            }

            using var json = JsonDocument.Parse(responseContent);

            var values = json.RootElement
                .GetProperty("embedding")
                .GetProperty("values");

            var embedding = values.EnumerateArray()
                .Select(value => value.GetSingle())
                .ToArray();

            if (embedding.Length != dimensions)
            {
                throw new InvalidOperationException(
                    $"Expected {dimensions} dimensions, received {embedding.Length}.");
            }

            return embedding;
        }
    }
}
