using RAGShowCaseService.Core.Entities;

namespace RAGShowCaseService.Core.Repositories;

public interface IPrivateDocumentRepository
{
    Task AddAsync(PrivateDocument privateDocument, CancellationToken cancellationToken = default);

    Task<PrivateDocument?> GetAsync(int documentId, CancellationToken cancellationToken = default);
}

