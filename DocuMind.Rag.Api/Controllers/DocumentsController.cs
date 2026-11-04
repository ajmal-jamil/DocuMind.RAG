using DocuMind.Application.Interfaces.Documents;
using DocuMind.Application.Interfaces.TextChunk;
using DocuMind.Infrastructure.Interface;
using DocuMind.Rag.Api.Contracts.Response;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.Rag.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {

        private readonly ITextChunker _textChunker;
        private readonly IDocumentTextExtractor _extractor;
        private readonly IDocumentStorageService _storageService;
        private readonly IDocumentSearchService _searchService;

        private readonly IDocumentAnswerService _answerService;

        public DocumentsController(IDocumentTextExtractor extractor, ITextChunker textChunker, IDocumentStorageService storageService, IDocumentSearchService searchService, IDocumentAnswerService answerService)
        {
            _extractor = extractor;
            _textChunker = textChunker;
            _storageService = storageService;
            _searchService = searchService;
            _answerService = answerService;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(IFormFile file,CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a file.");
            }

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            if (extension != ".pdf" && extension != ".txt")
            {
                return BadRequest("Only PDF and TXT files are supported.");
            }

            var extractedText = await _extractor.ExtractTextAsync(
                file, cancellationToken);

            var chunks = _textChunker.Chunk(extractedText);

            var documentId = await _storageService.SaveDocumentAsync(
                file.FileName,
                file.ContentType,
                chunks,
                cancellationToken);

            var response = new DocumentUploadResponse
            {
                DocumentId = documentId,
                FileName = Path.GetFileName(file.FileName),
                CharacterCount = extractedText.Length,
                ChunkCount = chunks.Count,
                Message = "Document and embeddings saved successfully."
            };

            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] string question, CancellationToken cancellationToken = default)
        {
            int topK = 5;
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question cannot be empty.");

            var results = await _searchService.SearchAsync(
                question,
                topK,
                cancellationToken);

            var response = new DocumentSearchResponse
            {
                Question = question,
                ResultCount = results.Count,
                Results = results.Select(x => new DocumentSearchResultResponse
                {
                    DocumentId = x.DocumentId,
                    FileName = x.FileName,
                    ChunkIndex = x.ChunkIndex,
                    Content = x.Content,
                    Distance = x.Distance
                }).ToList()
            };

            return Ok(response);
        }


        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string question, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question cannot be empty.");

            var answer = await _answerService.GenerateAnswerAsync(
                question,
                cancellationToken);

            var response = new DocumentAnswerResponse
            {
                Question = answer.Question,
                Answer = answer.Answer,
                Sources = answer.Sources.Select(x => new DocumentSearchResultResponse
                {
                    DocumentId = x.DocumentId,
                    FileName = x.FileName,
                    ChunkIndex = x.ChunkIndex,
                    Content = x.Content,
                    Distance = x.Distance
                }).ToList()
            };

            return Ok(response);
        }
    }
}
