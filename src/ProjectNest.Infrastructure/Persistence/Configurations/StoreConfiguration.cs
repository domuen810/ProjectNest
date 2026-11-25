using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Brands;
using ProjectNest.Domain.Malls;
using ProjectNest.Domain.Stores;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");
        builder.HasKey(store => store.Id);

        builder.Property(store => store.BrandId).IsRequired();
        builder.Property(store => store.MallId).IsRequired();
        builder.Property(store => store.Status).IsRequired();
        builder.Property(store => store.CreatedAt).IsRequired();
        builder.Property(store => store.UpdatedAt).IsRequired();

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(store => store.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Mall>()
            .WithMany()
            .HasForeignKey(store => store.MallId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
