namespace DocuMind.Application.Interfaces.Embeddings
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateDocumentEmbeddingAsync(string text, CancellationToken cancellationToken = default);

        Task<float[]> GenerateQueryEmbeddingAsync(string question, CancellationToken cancellationToken);

    }
}
