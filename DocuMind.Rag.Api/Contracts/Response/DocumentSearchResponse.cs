namespace DocuMind.Rag.Api.Contracts.Response
{
    public class DocumentSearchResponse
    {
        public string Question { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public List<DocumentSearchResultResponse> Results { get; set; } = new();
    }
}
