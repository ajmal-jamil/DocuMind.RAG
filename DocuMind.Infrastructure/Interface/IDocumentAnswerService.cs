using DocuMind.Application.Models;

namespace DocuMind.Infrastructure.Interface
{
    public interface IDocumentAnswerService
    {
        Task<DocumentAnswer> GenerateAnswerAsync(string question, CancellationToken cancellationToken = default);
    }
}
