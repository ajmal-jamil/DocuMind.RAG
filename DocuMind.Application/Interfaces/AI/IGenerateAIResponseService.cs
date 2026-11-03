namespace DocuMind.Application.Interfaces.AI
{
    public interface IGenerateAIResponseService
    {
        Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
