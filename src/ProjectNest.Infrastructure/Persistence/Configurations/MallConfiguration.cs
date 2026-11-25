using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Malls;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class MallConfiguration : IEntityTypeConfiguration<Mall>
{
    public void Configure(EntityTypeBuilder<Mall> builder)
    {
        builder.ToTable("Malls");
        builder.HasKey(mall => mall.Id);

        builder.Property(mall => mall.Name).IsRequired();
        builder.Property(mall => mall.City).IsRequired();
        builder.Property(mall => mall.Address).IsRequired();
        builder.Property(mall => mall.IsActive).IsRequired();
        builder.Property(mall => mall.CreatedAt).IsRequired();
        builder.Property(mall => mall.UpdatedAt).IsRequired();

        builder.HasIndex(mall => new { mall.City, mall.Name }).IsUnique();
    }
}
