using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RAGShowCaseService.Core.Entities;

namespace RAGShowCaseService.Infrastructure.Data.Configuration;

internal class PrivateDocumentConfiguration : IEntityTypeConfiguration<PrivateDocument>
{
    public void Configure(EntityTypeBuilder<PrivateDocument> builder)
    {
        builder.ToTable("private_documents");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
    }
}
