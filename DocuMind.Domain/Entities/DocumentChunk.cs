using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocuMind.Domain.Entities
{
    public class DocumentChunk
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DocumentId { get; set; }

        public int ChunkIndex { get; set; }

        public string Content { get; set; } = string.Empty;

        public int StartPosition { get; set; }

        public int EndPosition { get; set; }

        [Column(TypeName = "vector(768)")]
        public Vector? Embedding { get; set; }

        // Navigation property: this chunk belongs to one document.
        public Document Document { get; set; } = null!;
    }
}
