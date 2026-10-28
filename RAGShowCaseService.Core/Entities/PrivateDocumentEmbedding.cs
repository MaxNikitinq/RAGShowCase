using Pgvector;

namespace RAGShowCaseService.Core.Entities;

public class PrivateDocumentEmbedding
{
    public int Id { get; set; }

    public int PrivateDocumentId { get; set; }

    public required string Text { get; set; }

    public Vector? Embedding { get; set; }

    public int ChunkIndex { get; set; }

    public PrivateDocument? Document { get; set; }
}
