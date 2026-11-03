using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocuMind.Application.Interfaces.Embeddings
{
    public class GeminiEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiEmbeddingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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

            var apiKey = _configuration["AI:Embedding:ApiKey"];

            ArgumentException.ThrowIfNullOrEmpty(apiKey, "Gemini API key is not configured");

            var baseUrl = _configuration["AI:Embedding:BaseUrl"];
            var model = _configuration["AI:Embedding:Model"];
            int dimensions = int.TryParse(_configuration["AI:Embedding:Dimensions"], out var dim) ? dim : 768;


            ArgumentException.ThrowIfNullOrEmpty(baseUrl, "Gemini API base URL is not configured");
            ArgumentException.ThrowIfNullOrEmpty(model, "Gemini API model is not configured");
                        
            var url = $"{baseUrl}/{model}:embedContent";

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
