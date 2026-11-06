using DocuMind.Application.Interfaces.AI;
using DocuMind.Rag.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.Rag.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IGenerateAIResponseService _geminiChatService;
        private readonly ILogger<AiController> _logger;
        public AiController(IGenerateAIResponseService geminiChatService, ILogger<AiController> logger)
        {
            _geminiChatService = geminiChatService;
            _logger = logger;
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
                _logger.LogError(ex, "Error processing AI request");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
            }
        }
    }
    public record ChatRequest(string Prompt);

    public record ChatResponse(string Answer);
}
