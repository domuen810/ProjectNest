using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Brands;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(brand => brand.Id);

        builder.Property(brand => brand.Name).IsRequired();

        builder.Property(brand => brand.Code)
            .IsRequired();

        builder.Property(brand => brand.IsActive)
            .IsRequired();

        builder.Property(brand => brand.CreatedAt)
            .IsRequired();

        builder.Property(brand => brand.UpdatedAt)
            .IsRequired();

        builder.Property<string>("NormalizedName")
            .HasComputedColumnSql("lower(regexp_replace(\"Name\", '^[[:space:]]+|[[:space:]]+$', '', 'g'))", stored: true);

        builder.HasIndex("NormalizedName")
            .IsUnique();
    }
}
