using JasperDocs.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JasperDocs.WebApi.Infrastructure.EntityConfigurations;

public class DocumentVersionEntityTypeConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder
            .HasIndex(e => new { e.DocumentId, e.VersionNumber })
            .IsUnique();

        builder
            .Property(e => e.DocumentId)
            .IsRequired();

        builder
            .Property(e => e.VersionNumber)
            .IsRequired();

        builder
            .Property(e => e.StoragePath)
            .IsRequired();

        builder
            .HasOne(e => e.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .ValueGeneratedOnAdd();
    }
}
