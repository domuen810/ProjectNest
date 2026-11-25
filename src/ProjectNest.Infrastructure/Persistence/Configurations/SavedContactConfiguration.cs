using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Brands;
using ProjectNest.Domain.Contacts;
using ProjectNest.Domain.Malls;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class SavedContactConfiguration : IEntityTypeConfiguration<SavedContact>
{
    public void Configure(EntityTypeBuilder<SavedContact> builder)
    {
        builder.ToTable("SavedContacts", table =>
        {
            table.HasCheckConstraint(
                "CK_SavedContacts_ExactlyOneOwner",
                "(\"BrandId\" IS NOT NULL AND \"MallId\" IS NULL) OR (\"BrandId\" IS NULL AND \"MallId\" IS NOT NULL)");
        });
        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Name).IsRequired();
        builder.Property(contact => contact.IsActive).IsRequired();
        builder.Property(contact => contact.CreatedAt).IsRequired();
        builder.Property(contact => contact.UpdatedAt).IsRequired();

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(contact => contact.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Mall>()
            .WithMany()
            .HasForeignKey(contact => contact.MallId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
