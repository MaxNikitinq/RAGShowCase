using Microsoft.EntityFrameworkCore;
using RAGShowCaseService.Application.Abstractions.UnitOfWork;
using RAGShowCaseService.Core.Entities;

namespace RAGShowCaseService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<PrivateDocument> PrivateFiles { get; set; }
    public DbSet<PrivateDocumentEmbedding> PrivateFileEmbeddings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // converts to "CREATE EXTENSION IF NOT EXISTS vector;"
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
