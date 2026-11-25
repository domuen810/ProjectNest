using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Files;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Projects;
using ProjectNest.Domain.Surveys;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class SurveyWorkOrderConfiguration : IEntityTypeConfiguration<SurveyWorkOrder>
{
    public void Configure(EntityTypeBuilder<SurveyWorkOrder> builder)
    {
        builder.ToTable("SurveyWorkOrders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.ProjectId).IsRequired();
        builder.Property(order => order.SurveyType).IsRequired();
        builder.Property(order => order.Status).IsRequired();
        builder.Property(order => order.CreatedByUserId).IsRequired();
        builder.Property(order => order.CreatedAt).IsRequired();
        builder.Property(order => order.UpdatedByUserId).IsRequired();
        builder.Property(order => order.UpdatedAt).IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(order => order.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StoredFile>()
            .WithMany()
            .HasForeignKey(order => order.FinalSurveyReportFileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Visits)
            .WithOne()
            .HasForeignKey(visit => visit.SurveyWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
