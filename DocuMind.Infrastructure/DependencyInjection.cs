using DocuMind.Application.Interfaces.AI;
using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Application.Services.Documents;
using DocuMind.Infrastructure.AI;
using DocuMind.Infrastructure.Interface;
using DocuMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocuMind.Infrastructure.AI.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DocuMind.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddScoped<IDocumentSearchService, DocumentSearchService>();
            services.AddScoped<IDocumentAnswerService, DocumentAnswerService>();
            services.AddScoped<IDocumentStorageService, DocumentStorageService>();

            services.AddOptions<ChatOptions>()
                .Bind(configuration.GetSection(ChatOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart(); 
            services.AddOptions<EmbeddingOptions>()
                .Bind(configuration.GetSection(EmbeddingOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var connectionString =
            configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions => npgsqlOptions.UseVector());
            });

            return services;
        }
    }
}
