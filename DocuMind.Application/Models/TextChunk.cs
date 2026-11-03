namespace DocuMind.Application.Models
{
    public class TextChunk
    {
        public int Index { get; set; }

        public string Content { get; set; } = string.Empty;

        public int StartPosition { get; set; }

        public int EndPosition { get; set; }
    }
}
