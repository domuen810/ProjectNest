using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Projects;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
{
    public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
    {
        builder.ToTable("ProjectAssignments");
        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.ProjectId).IsRequired();
        builder.Property(assignment => assignment.DesignerUserId).IsRequired();
        builder.Property(assignment => assignment.AssignedAt).IsRequired();
        builder.Property(assignment => assignment.AssignedByUserId).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(assignment => assignment.DesignerUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(assignment => assignment.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
