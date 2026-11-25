using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Identity;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.ToTable("JobPositions");
        builder.HasKey(position => position.Id);

        builder.Property(position => position.Code).IsRequired();
        builder.Property(position => position.Name).IsRequired();
    }
}
