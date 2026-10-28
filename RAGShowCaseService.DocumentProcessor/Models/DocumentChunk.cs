namespace RAGShowCaseService.DocumentProcessor.Models;

public class DocumentChunk
{
    public int DocumentId { get; set; }

    public required string Text { get; set; }

    public float[]? Embedding { get; set; }

    public int ChunkIndex { get; set; }
}
