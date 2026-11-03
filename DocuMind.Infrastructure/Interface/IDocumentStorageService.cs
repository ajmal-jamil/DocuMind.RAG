using DocuMind.Application.Models;

namespace DocuMind.Infrastructure.Interface
{
    public interface IDocumentStorageService
    {
        Task<Guid> SaveDocumentAsync(
        string fileName,
        string contentType,
        List<TextChunk> chunks,
        CancellationToken cancellationToken = default);
    }
}
