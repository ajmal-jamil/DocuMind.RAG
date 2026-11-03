using DocuMind.Application.Interfaces.AI;
using DocuMind.Rag.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.Rag.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IGenerateAIResponseService _geminiChatService;

        public AiController(IGenerateAIResponseService geminiChatService)
        {
            _geminiChatService = geminiChatService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }
            try
            {
                var response = await _geminiChatService.GenerateResponseAsync(request.Prompt, cancellationToken);
                return Ok(new ChatResponse(response));
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
    public record ChatRequest(string Prompt);

    public record ChatResponse(string Answer);
}
