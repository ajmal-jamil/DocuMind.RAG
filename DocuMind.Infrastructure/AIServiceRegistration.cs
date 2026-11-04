using DocuMind.Application.Interfaces.AI;
using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Infrastructure.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocuMind.Infrastructure
{
    public static class AIServiceRegistration
    {
        public static IServiceCollection AddAIServices(this IServiceCollection services,IConfiguration configuration)
        {
            var chatProvider = configuration["AI:Chat:Provider"];
            var embeddingProvider = configuration["AI:Embedding:Provider"];

            switch (chatProvider?.Trim().ToLowerInvariant())
            {
                case "gemini":
                    AddResilience(services.AddHttpClient<IGenerateAIResponseService, GeminiChatService>());
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported chat provider: {chatProvider}");
            }

            switch (embeddingProvider?.Trim().ToLowerInvariant())
            {
                case "gemini":
                    AddResilience(services.AddHttpClient<IEmbeddingService, GeminiEmbeddingService>());
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported embedding provider: {embeddingProvider}");
            }

            return services;
        }

        private static void AddResilience(IHttpClientBuilder httpClientBuilder)
        {
            httpClientBuilder.AddStandardResilienceHandler(o =>
            {
                o.Retry.MaxRetryAttempts = 3;
                o.Retry.Delay = TimeSpan.FromSeconds(1);
                o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

                o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
            });
        }
    }
}