using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Files;
using ProjectNest.Domain.Surveys;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class SurveyVisitFileConfiguration : IEntityTypeConfiguration<SurveyVisitFile>
{
    public void Configure(EntityTypeBuilder<SurveyVisitFile> builder)
    {
        builder.ToTable("SurveyVisitFiles");
        builder.HasKey(file => file.Id);

        builder.Property(file => file.SurveyVisitId).IsRequired();
        builder.Property(file => file.StoredFileId).IsRequired();
        builder.Property(file => file.Category).IsRequired();

        builder.HasOne(file => file.StoredFile)
            .WithMany()
            .HasForeignKey(file => file.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
