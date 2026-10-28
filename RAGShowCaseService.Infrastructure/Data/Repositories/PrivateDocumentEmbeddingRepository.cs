using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using RAGShowCaseService.Core.Entities;
using RAGShowCaseService.Core.Repositories;

namespace RAGShowCaseService.Infrastructure.Data.Repositories;

internal class PrivateDocumentEmbeddingRepository : IPrivateDocumentEmbeddingRepository
{
    private readonly ApplicationDbContext _context;

    public PrivateDocumentEmbeddingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> SearchByCosineDistanceAsync(Vector embedding, int amount = 5, CancellationToken cancellationToken = default)
    {
        var results = await _context.PrivateFileEmbeddings
            .Include(x => x.Document)
            .Where(x => x.Document != null && x.Document.Status == PrivateFileStatus.Processed)
            .OrderBy(x => x.Embedding!.CosineDistance(embedding))
            .Take(amount)
            .Select(x => x.Text)
            .ToListAsync(cancellationToken);

        return results;
    }
}
