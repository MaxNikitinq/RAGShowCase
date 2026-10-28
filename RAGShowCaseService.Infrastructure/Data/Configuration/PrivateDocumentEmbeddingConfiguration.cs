using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RAGShowCaseService.Core.Entities;

namespace RAGShowCaseService.Infrastructure.Data.Configuration;

internal class PrivateDocumentEmbeddingConfiguration : IEntityTypeConfiguration<PrivateDocumentEmbedding>
{
    public void Configure(EntityTypeBuilder<PrivateDocumentEmbedding> builder)
    {
        builder.ToTable("private_document_embeddings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PrivateDocumentId).HasColumnName("private_document_id");
        builder.Property(x => x.Text).HasColumnName("text").HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ChunkIndex).HasColumnName("chunk_index");
        builder.Property(x => x.Embedding).HasColumnName("embedding").HasColumnType("vector(1024)");

        builder.HasIndex(x => x.Embedding).HasMethod("ivfflat").HasOperators("vector_cosine_ops");

        builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.PrivateDocumentId);
    }
}
