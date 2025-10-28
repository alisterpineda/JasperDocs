using JasperDocs.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JasperDocs.WebApi.Infrastructure.EntityConfigurations;

public class PartyEntityTypeConfiguration : IEntityTypeConfiguration<Party>
{
    public void Configure(EntityTypeBuilder<Party> builder)
    {
        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

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

        builder
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .ValueGeneratedOnAddOrUpdate();
    }
}
