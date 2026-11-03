using DocuMind.Application.Interfaces.Documents;
using DocuMind.Infrastructure.Interface;
using DocuMind.Domain.Entities;
using DocuMind.Application.Interfaces.Embeddings;
using Pgvector;
using DocuMind.Infrastructure.Persistence;

namespace DocuMind.Application.Services.Documents
{
    public class DocumentStorageService : IDocumentStorageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmbeddingService _embeddingService;

        public DocumentStorageService(IEmbeddingService embeddingService, ApplicationDbContext dbContext)
        {
            _embeddingService = embeddingService;
            _dbContext = dbContext;
        }

        public async Task<Guid> SaveDocumentAsync(string fileName, string contentType, List<Models.TextChunk> chunks, CancellationToken cancellationToken = default)
        {

            var document = new Document
            {
                FileName = fileName,
                ContentType = contentType,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var chunk in chunks)
            {
                var embeddingValues =
                    await _embeddingService.GenerateDocumentEmbeddingAsync(
                        chunk.Content,
                        cancellationToken);

                if (embeddingValues.Length != 768)
                {
                    throw new InvalidOperationException(
                        $"Expected 768 embedding dimensions, but received {embeddingValues.Length}.");
                }

                document.Chunks.Add(new DocumentChunk
                {
                    ChunkIndex = chunk.Index,
                    Content = chunk.Content,
                    StartPosition = chunk.StartPosition,
                    EndPosition = chunk.EndPosition,
                    Embedding = new Vector(embeddingValues)
                });
            }

            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return document.Id;
        }
    }
}
