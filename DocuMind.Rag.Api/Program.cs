using DocuMind.Rag.Api.Services;
using DocuMind.Rag.Api.Services.TextChunk;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using DocuMind.Application.Interfaces.AI;
using DocuMind.Application.Interfaces.Documents;
using DocuMind.Application.Interfaces.TextChunk;
using DocuMind.Infrastructure.Interface;
using DocuMind.Application.Services.Documents;
using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Application.Services.AI;
using DocuMind.Application;
using DocuMind.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
