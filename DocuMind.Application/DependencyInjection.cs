using DocuMind.Application.Interfaces.Documents;
using DocuMind.Application.Interfaces.TextChunk;
using DocuMind.Application.Services.Documents;
using DocuMind.Rag.Api.Services.TextChunk;
using Microsoft.Extensions.DependencyInjection;

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

            
            return services;
        }
    }
}
