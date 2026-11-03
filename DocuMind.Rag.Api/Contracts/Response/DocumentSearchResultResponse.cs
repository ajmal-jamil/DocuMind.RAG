namespace DocuMind.Rag.Api.Contracts.Response
{
    public class DocumentSearchResultResponse
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public string Content { get; set; } = string.Empty;
        public double Distance { get; set; }
    }
}
