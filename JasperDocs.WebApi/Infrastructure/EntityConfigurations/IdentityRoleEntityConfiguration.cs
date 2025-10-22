using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JasperDocs.WebApi.Infrastructure.EntityConfigurations;

public class IdentityRoleEntityConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
            }
        );
    }
}
