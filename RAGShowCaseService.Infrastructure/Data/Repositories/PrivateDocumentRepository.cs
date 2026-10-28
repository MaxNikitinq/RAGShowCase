using Microsoft.EntityFrameworkCore;
using RAGShowCaseService.Core.Entities;
using RAGShowCaseService.Core.Repositories;

namespace RAGShowCaseService.Infrastructure.Data.Repositories;

internal class PrivateDocumentRepository : IPrivateDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public PrivateDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PrivateDocument privateDocument, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(privateDocument, cancellationToken);
    }

    public async Task<PrivateDocument?> GetAsync(int documentId, CancellationToken cancellationToken = default)
    {
        return await _context.PrivateFiles.FirstOrDefaultAsync(x => x.Id == documentId, cancellationToken);
    }
}
