using RAGShowCaseService.DocumentProcessor.Models;

namespace RAGShowCaseService.DocumentProcessor.Services.Abstractions;

public interface IChunkerService
{
    List<DocumentChunk> PrepareChunks(string documentContent, int documentId);
}
