namespace DocuMind.Application.Models
{
    public class DocumentAnswer
    {
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public List<DocumentSearchResult> Sources { get; set; } = new();
    }
}
