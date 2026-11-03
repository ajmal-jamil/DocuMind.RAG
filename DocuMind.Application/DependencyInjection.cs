using DocuMind.Application.Interfaces.AI;
using DocuMind.Application.Interfaces.Documents;
using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Application.Interfaces.TextChunk;
using DocuMind.Application.Services.AI;
using DocuMind.Application.Services.Documents;
using DocuMind.Rag.Api.Services.TextChunk;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocuMind.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services)
        {
            // Application service registrations will go here.

            services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();
            services.AddScoped<ITextChunker, TextChunker>();

            services.AddHttpClient<IGenerateAIResponseService, GeminiChatService>().AddStandardResilienceHandler(o =>
            {
                // Retries 429, 408, 5xx and transient network errors; honours Retry-After by default.
                o.Retry.MaxRetryAttempts = 3;
                o.Retry.Delay = TimeSpan.FromSeconds(1);
                o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
            });
            services.AddHttpClient<IEmbeddingService, GeminiEmbeddingService>().AddStandardResilienceHandler(o =>
            {
                // Retries 429, 408, 5xx and transient network errors; honours Retry-After by default.
                o.Retry.MaxRetryAttempts = 3;
                o.Retry.Delay = TimeSpan.FromSeconds(1);
                o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
                o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
                o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
                o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);
            });
            return services;
        }
    }
}
