using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Production;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProductionProgressHistoryConfiguration : IEntityTypeConfiguration<ProductionProgressHistory>
{
    public void Configure(EntityTypeBuilder<ProductionProgressHistory> builder)
    {
        builder.ToTable("ProductionProgressHistory");
        builder.HasKey(history => history.Id);

        builder.Property(history => history.ProductionStageProgressId).IsRequired();
        builder.Property(history => history.PreviousStatus).IsRequired();
        builder.Property(history => history.NewStatus).IsRequired();
        builder.Property(history => history.ChangedAt).IsRequired();
        builder.Property(history => history.ChangedByUserId).IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(history => history.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
