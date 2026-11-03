using DocuMind.Application.Interfaces.TextChunk;

namespace DocuMind.Rag.Api.Services.TextChunk
{
    public class TextChunker : ITextChunker
    {
        private const int ChunkSize = 1000;
        private const int OverlapSize = 150;

        public List<Application.Models.TextChunk> Chunk(string text)
        {
            var chunks = new List<Application.Models.TextChunk>();

            if (string.IsNullOrWhiteSpace(text))
                return chunks;

            text = text.Trim();

            int start = 0;
            int index = 0;

            while (start < text.Length)
            {
                int end = Math.Min(start + ChunkSize, text.Length);

                // Try to avoid splitting in the middle of a word.
                if (end < text.Length)
                {
                    int lastSpace = text.LastIndexOf(' ', end - 1, end - start);

                    if (lastSpace > start + ChunkSize / 2)
                        end = lastSpace;
                }

                string content = text[start..end].Trim();

                if (!string.IsNullOrWhiteSpace(content))
                {
                    chunks.Add(new Application.Models.TextChunk
                    {
                        Index = index,
                        Content = content,
                        StartPosition = start,
                        EndPosition = end
                    });

                    index++;
                }

                if (end >= text.Length)
                    break;

                start = end - OverlapSize;
            }

            return chunks;

        }
    }
}
