using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Files;
using ProjectNest.Domain.Identity;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("StoredFiles");
        builder.HasKey(file => file.Id);

        builder.Property(file => file.OriginalFileName).IsRequired();
        builder.Property(file => file.ContentType).IsRequired();
        builder.Property(file => file.FileExtension).IsRequired();
        builder.Property(file => file.FileSize).IsRequired();
        builder.Property(file => file.StorageKey).IsRequired();
        builder.Property(file => file.UploadedAt).IsRequired();
        builder.Property(file => file.UploadedById).IsRequired();

        builder.HasIndex(file => file.StorageKey).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(file => file.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
