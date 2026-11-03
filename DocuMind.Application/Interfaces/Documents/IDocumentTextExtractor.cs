using Microsoft.AspNetCore.Http;

namespace DocuMind.Application.Interfaces.Documents
{
    public interface IDocumentTextExtractor
    {
        Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
