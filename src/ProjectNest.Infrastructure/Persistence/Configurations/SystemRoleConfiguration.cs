using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class SystemRoleConfiguration : IEntityTypeConfiguration<SystemRole>
{
    public void Configure(EntityTypeBuilder<SystemRole> builder)
    {
        builder.ToTable("SystemRoles");
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Code).IsRequired();
        builder.Property(role => role.Name).IsRequired();
    }
}
