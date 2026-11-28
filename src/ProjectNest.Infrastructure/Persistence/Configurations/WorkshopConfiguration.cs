using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Production;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");
        builder.HasKey(workshop => workshop.Id);

        builder.Property(workshop => workshop.Name).IsRequired();
        builder.Property(workshop => workshop.IsActive).IsRequired();
    }
}
