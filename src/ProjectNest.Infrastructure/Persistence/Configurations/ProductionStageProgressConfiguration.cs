using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Production;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProductionStageProgressConfiguration : IEntityTypeConfiguration<ProductionStageProgress>
{
    public void Configure(EntityTypeBuilder<ProductionStageProgress> builder)
    {
        builder.ToTable("ProductionStageProgress");
        builder.HasKey(progress => progress.Id);

        builder.Property(progress => progress.ProductionOrderId).IsRequired();
        builder.Property(progress => progress.WorkshopId).IsRequired();
        builder.Property(progress => progress.Status).IsRequired();

        builder.HasIndex(progress => new { progress.ProductionOrderId, progress.WorkshopId })
            .IsUnique();

        builder.HasOne<ProductionOrder>()
            .WithMany()
            .HasForeignKey(progress => progress.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Workshop>()
            .WithMany()
            .HasForeignKey(progress => progress.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(progress => progress.StatusHistory)
            .WithOne()
            .HasForeignKey(history => history.ProductionStageProgressId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(progress => progress.ReworkHistory)
            .WithOne()
            .HasForeignKey(record => record.ProductionStageProgressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
