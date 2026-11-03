using DocuMind.Application.Interfaces.AI;
using DocuMind.Application.Interfaces.Documents;
using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Application.Services.AI;
using DocuMind.Application.Services.Documents;
using DocuMind.Infrastructure.Interface;
using DocuMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
