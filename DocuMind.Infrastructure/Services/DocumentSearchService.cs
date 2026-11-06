using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Application.Models;
using DocuMind.Infrastructure.Interface;
using DocuMind.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;


namespace DocuMind.Infrastructure.Interface
{
    public class DocumentSearchService : IDocumentSearchService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmbeddingService _embeddingService;

        public DocumentSearchService(
            ApplicationDbContext dbContext,
            IEmbeddingService embeddingService)
        {
            _dbContext = dbContext;
            _embeddingService = embeddingService;
        }

        public async Task<List<DocumentSearchResult>> SearchAsync(
            string question,
            int topK = 5,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException("Question cannot be empty.");

            if (topK < 1 || topK > 20)
                throw new ArgumentOutOfRangeException(
                    nameof(topK), "TopK must be between 1 and 20.");

            // 1. Convert the user's question into an embedding.
            var embeddingValues =
                await _embeddingService.GenerateQueryEmbeddingAsync(
                    question, cancellationToken);

            if (embeddingValues.Length != 768)
            {
                throw new InvalidOperationException(
                    $"Expected 768 dimensions, received {embeddingValues.Length}.");
            }

            var queryVector = new Vector(embeddingValues);
            var maxDistance = 0.45; // Cosine distance ranges from 0 (identical) to 2 (opposite)
            // 2. Find the closest document chunks using cosine distance.
            var results = await _dbContext.DocumentChunks
                .AsNoTracking()
                .Where(chunk => chunk.Embedding != null)
                .OrderBy(chunk =>
                    chunk.Embedding!.CosineDistance(queryVector))
                .Select(chunk => new DocumentSearchResult
                {
                    DocumentId = chunk.DocumentId,
                    FileName = chunk.Document.FileName,
                    ChunkIndex = chunk.ChunkIndex,
                    Content = chunk.Content,
                    Distance = chunk.Embedding!.CosineDistance(queryVector)
                })
                .Where(x => x.Distance <= maxDistance)
                .Take(topK)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
