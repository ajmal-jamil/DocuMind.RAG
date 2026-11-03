namespace DocuMind.Rag.Api.Contracts.Response
{
    public class EmbeddingResponse
    {
        public int Dimensions { get; set; }

        public float[] Values { get; set; } = [];
    }
}
