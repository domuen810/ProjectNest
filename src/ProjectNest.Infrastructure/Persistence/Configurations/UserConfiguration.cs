using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name).IsRequired();
        builder.Property(user => user.Email).IsRequired();
        builder.Property(user => user.JobPositionId).IsRequired();
        builder.Property(user => user.IsActive).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
        builder.Property(user => user.UpdatedAt).IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();

        builder.HasOne<JobPosition>()
            .WithMany()
            .HasForeignKey(user => user.JobPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(user => user.SystemRoles)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UserSystemRoles",
                role => role.HasOne<SystemRole>()
                    .WithMany()
                    .HasForeignKey("SystemRoleId")
                    .OnDelete(DeleteBehavior.Restrict),
                user => user.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("UserSystemRoles");
                    join.HasKey("UserId", "SystemRoleId");
                });
    }
}
