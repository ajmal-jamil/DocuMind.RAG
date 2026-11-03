namespace DocuMind.Rag.Api.Contracts.Response
{
    public class DocumentUploadResponse
    {
        public Guid DocumentId { get; set; } = Guid.Empty;

        public string FileName { get; set; } = string.Empty;

        public int CharacterCount { get; set; } = 0;

        public int ChunkCount { get; set; } = 0;

        public string Message { get; set; } = string.Empty;


    }
}
