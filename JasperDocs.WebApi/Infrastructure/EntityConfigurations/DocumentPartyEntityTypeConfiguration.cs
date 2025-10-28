using JasperDocs.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JasperDocs.WebApi.Infrastructure.EntityConfigurations;

public class DocumentPartyEntityTypeConfiguration : IEntityTypeConfiguration<DocumentParty>
{
    public void Configure(EntityTypeBuilder<DocumentParty> builder)
    {
        builder
            .HasKey(e => new { e.DocumentId, e.PartyId });

        builder
            .HasOne(e => e.Document)
            .WithMany(d => d.DocumentParties)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasOne(e => e.Party)
            .WithMany(p => p.DocumentParties)
            .HasForeignKey(e => e.PartyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .ValueGeneratedOnAdd();
    }
}
