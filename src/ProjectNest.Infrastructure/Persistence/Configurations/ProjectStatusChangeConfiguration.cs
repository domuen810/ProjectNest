using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Projects;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProjectStatusChangeConfiguration : IEntityTypeConfiguration<ProjectStatusChange>
{
    public void Configure(EntityTypeBuilder<ProjectStatusChange> builder)
    {
        builder.ToTable("ProjectStatusChanges", table =>
        {
            table.HasCheckConstraint(
                "CK_ProjectStatusChanges_ReasonForPausedOrCancelled",
                $"\"NewStatus\" NOT IN ({(int)ProjectStatus.Paused}, {(int)ProjectStatus.Cancelled}) OR (\"Reason\" IS NOT NULL AND regexp_replace(\"Reason\", '^[[:space:]]+|[[:space:]]+$', '', 'g') <> '')");
        });
        builder.HasKey(change => change.Id);

        builder.Property(change => change.ProjectId).IsRequired();
        builder.Property(change => change.PreviousStatus).IsRequired();
        builder.Property(change => change.NewStatus).IsRequired();
        builder.Property(change => change.ChangedAt).IsRequired();
        builder.Property(change => change.ChangedByUserId).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(change => change.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
