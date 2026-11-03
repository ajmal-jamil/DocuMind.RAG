using DocuMind.Application.Interfaces.Embeddings;
using DocuMind.Rag.Api.Contracts.Request;
using DocuMind.Rag.Api.Contracts.Response;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.Rag.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbeddingsController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;

        public EmbeddingsController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }


        [HttpPost("test-embedding")]
        public async Task<IActionResult> Test(EmbeddingRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required.");

            var vector = await _embeddingService.GenerateDocumentEmbeddingAsync(request.Text, cancellationToken);

            var result = new EmbeddingResponse
            {
                Dimensions = vector.Length,
                Values = vector
            };

            return Ok(result);
        }
    }
}
