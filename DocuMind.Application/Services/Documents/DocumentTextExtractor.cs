using DocuMind.Application.Interfaces.Documents;
using DocuMind.Rag.Api.Services.TextChunk;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace DocuMind.Application.Services.Documents
{
    public class DocumentTextExtractor : IDocumentTextExtractor
    {
        

        public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException("The uploaded file is empty.");
            }

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            if (extension == ".txt")
            {
                using var reader = new StreamReader(file.OpenReadStream());

                return await reader.ReadToEndAsync(cancellationToken);
            }

            if (extension == ".pdf")
            {
                await using var memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream, cancellationToken);

                memoryStream.Position = 0;

                using var pdf = PdfDocument.Open(memoryStream);

                var text = string.Join(
                    Environment.NewLine,
                    pdf.GetPages().Select(page => page.Text));

                return text;
            }

            throw new NotSupportedException(
                "Only PDF and TXT files are supported.");
        }
    }
}
