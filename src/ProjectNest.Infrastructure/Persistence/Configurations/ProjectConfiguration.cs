using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Projects;
using ProjectNest.Domain.Stores;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", table =>
        {
            table.HasCheckConstraint(
                "CK_Projects_PaymentShares",
                "\"BrandPaymentPercentage\" BETWEEN 0 AND 100 AND \"MallPaymentPercentage\" BETWEEN 0 AND 100 AND \"BrandPaymentPercentage\" + \"MallPaymentPercentage\" = 100");
        });
        builder.HasKey(project => project.Id);

        builder.Property(project => project.ProjectNumber).IsRequired();
        builder.Property(project => project.StoreId).IsRequired();
        builder.Property(project => project.ProjectNature).IsRequired();
        builder.Property(project => project.ProjectCategory).IsRequired();
        builder.Property(project => project.OpeningDate).IsRequired();
        builder.Property(project => project.BrandPaymentPercentage).IsRequired();
        builder.Property(project => project.MallPaymentPercentage).IsRequired();
        builder.Property(project => project.BrandNameSnapshot).IsRequired();
        builder.Property(project => project.MallNameSnapshot).IsRequired();
        builder.Property(project => project.RequiresTemporaryCounter).IsRequired();
        builder.Property(project => project.RequiresExistingCounterRemoval).IsRequired();
        builder.Property(project => project.Status).IsRequired();
        builder.Property(project => project.CreatedByUserId).IsRequired();
        builder.Property(project => project.UpdatedByUserId).IsRequired();
        builder.Property(project => project.CreatedAt).IsRequired();
        builder.Property(project => project.UpdatedAt).IsRequired();

        builder.HasIndex(project => project.ProjectNumber).IsUnique();

        builder.HasOne<Store>()
            .WithMany()
            .HasForeignKey(project => project.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(project => project.ResponsibleDesignerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(project => project.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(project => project.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(project => project.ResponsibleDesignerAssignmentHistory)
            .WithOne()
            .HasForeignKey(assignment => assignment.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(project => project.StatusHistory)
            .WithOne()
            .HasForeignKey(change => change.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(project => project.OpeningDateHistory, history =>
        {
            history.ToTable("OpeningDateChanges");
            history.WithOwner().HasForeignKey("ProjectId");
            history.Property<Guid>("Id").ValueGeneratedOnAdd();
            history.HasKey("ProjectId", "Id");
            history.Property(change => change.PreviousValue).IsRequired();
            history.Property(change => change.NewValue).IsRequired();
            history.Property(change => change.ChangedByUserId).IsRequired();
            history.Property(change => change.ChangedAt).IsRequired();
            history.HasOne<User>()
                .WithMany()
                .HasForeignKey(change => change.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.OwnsMany(project => project.MeasuredAreaHistory, history =>
        {
            history.ToTable("MeasuredAreaChanges");
            history.WithOwner().HasForeignKey("ProjectId");
            history.Property<Guid>("Id").ValueGeneratedOnAdd();
            history.HasKey("ProjectId", "Id");
            history.Property(change => change.PreviousValue);
            history.Property(change => change.NewValue);
            history.Property(change => change.ChangedByUserId).IsRequired();
            history.Property(change => change.ChangedAt).IsRequired();
            history.Property(change => change.Reason).IsRequired();
            history.HasOne<User>()
                .WithMany()
                .HasForeignKey(change => change.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
