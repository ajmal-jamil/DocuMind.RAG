namespace DocuMind.Application.Interfaces.TextChunk
{
    public interface ITextChunker
    {
        List<Application.Models.TextChunk> Chunk(string text);
    }
}
