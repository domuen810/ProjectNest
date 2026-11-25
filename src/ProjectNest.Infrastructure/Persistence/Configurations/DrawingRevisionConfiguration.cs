using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Projects;
using ProjectNest.Domain.Drawings;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class DrawingRevisionConfiguration : IEntityTypeConfiguration<DrawingRevision>
{
    public void Configure(EntityTypeBuilder<DrawingRevision> builder)
    {
        builder.ToTable("DrawingRevisions");
        builder.HasKey(revision => revision.Id);

        builder.Property(revision => revision.ProjectId).IsRequired();
        builder.Property(revision => revision.DrawingStage).IsRequired();
        builder.Property(revision => revision.RevisionNumber).IsRequired();
        builder.Property(revision => revision.ChangeScope).IsRequired();
        builder.Property(revision => revision.ChangeReason).IsRequired();
        builder.Property(revision => revision.IsCurrent).IsRequired();
        builder.Property(revision => revision.CreatedByUserId).IsRequired();
        builder.Property(revision => revision.CreatedAt).IsRequired();

        builder.HasIndex(revision => new { revision.ProjectId, revision.DrawingStage, revision.RevisionNumber })
            .IsUnique();
        builder.HasIndex(revision => new { revision.ProjectId, revision.DrawingStage })
            .IsUnique()
            .HasFilter("\"IsCurrent\" = TRUE");

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(revision => revision.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(revision => revision.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(revision => revision.FileAssociations)
            .WithOne()
            .HasForeignKey(file => file.DrawingRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
