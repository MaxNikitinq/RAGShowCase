using Pgvector;

namespace RAGShowCaseService.Core.Repositories;

public interface IPrivateDocumentEmbeddingRepository
{
    Task<List<string>> SearchByCosineDistanceAsync(Vector embedding, int amount = 5, CancellationToken cancellationToken = default);
}
