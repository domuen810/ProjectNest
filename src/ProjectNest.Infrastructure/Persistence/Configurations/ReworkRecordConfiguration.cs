using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Production;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ReworkRecordConfiguration : IEntityTypeConfiguration<ReworkRecord>
{
    public void Configure(EntityTypeBuilder<ReworkRecord> builder)
    {
        builder.ToTable("ReworkRecords");
        builder.HasKey(record => record.Id);

        builder.Property(record => record.ProductionStageProgressId).IsRequired();
        builder.Property(record => record.ReasonType).IsRequired();
        builder.Property(record => record.RecordedByUserId).IsRequired();
        builder.Property(record => record.RecordedAt).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(record => record.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
