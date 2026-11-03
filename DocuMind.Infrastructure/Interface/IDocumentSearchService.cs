using DocuMind.Application.Models;

namespace DocuMind.Infrastructure.Interface
{
    public interface IDocumentSearchService
    {
        Task<List<DocumentSearchResult>> SearchAsync(
        string question,
        int topK = 5,
        CancellationToken cancellationToken = default);
    }
}
