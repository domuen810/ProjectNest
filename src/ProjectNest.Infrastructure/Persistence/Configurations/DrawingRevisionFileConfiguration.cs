using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Drawings;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class DrawingRevisionFileConfiguration : IEntityTypeConfiguration<DrawingRevisionFile>
{
    public void Configure(EntityTypeBuilder<DrawingRevisionFile> builder)
    {
        builder.ToTable("DrawingRevisionFiles");
        builder.HasKey(file => file.Id);

        builder.Property(file => file.DrawingRevisionId).IsRequired();
        builder.Property(file => file.StoredFileId).IsRequired();
        builder.Property(file => file.FileRole).IsRequired();

        builder.HasOne(file => file.StoredFile)
            .WithMany()
            .HasForeignKey(file => file.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
