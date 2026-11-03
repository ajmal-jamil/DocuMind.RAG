namespace DocuMind.Rag.Api.Contracts.Response
{
    public class DocumentAnswerResponse
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<DocumentSearchResultResponse> Sources { get; set; } = new();
    }
}
