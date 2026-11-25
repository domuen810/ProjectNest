using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Files;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Surveys;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class SurveyVisitConfiguration : IEntityTypeConfiguration<SurveyVisit>
{
    public void Configure(EntityTypeBuilder<SurveyVisit> builder)
    {
        builder.ToTable("SurveyVisits");
        builder.HasKey(visit => visit.Id);

        builder.Property(visit => visit.SurveyWorkOrderId).IsRequired();
        builder.Property(visit => visit.PlannedDate).IsRequired();
        builder.Property(visit => visit.Status).IsRequired();
        builder.Property(visit => visit.CreatedAt).IsRequired();
        builder.Property(visit => visit.UpdatedAt).IsRequired();

        builder.HasOne<StoredFile>()
            .WithMany()
            .HasForeignKey(visit => visit.VisitReportFileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(visit => visit.ConfirmedByDesignerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(visit => visit.Personnel, personnel =>
        {
            personnel.ToTable("SurveyPersonnel");
            personnel.WithOwner().HasForeignKey("SurveyVisitId");
            personnel.Property<Guid>("Id").ValueGeneratedOnAdd();
            personnel.HasKey("SurveyVisitId", "Id");
            personnel.Property(person => person.Name).IsRequired();
            personnel.Property(person => person.Phone).IsRequired();
        });

        builder.HasMany(visit => visit.Files)
            .WithOne()
            .HasForeignKey(file => file.SurveyVisitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
