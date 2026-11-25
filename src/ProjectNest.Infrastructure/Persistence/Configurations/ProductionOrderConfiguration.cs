using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectNest.Domain.Drawings;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Production;
using ProjectNest.Domain.Projects;

namespace ProjectNest.Infrastructure.Persistence.Configurations;

public sealed class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.ToTable("ProductionOrders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.ProjectId).IsRequired();
        builder.Property(order => order.OrderNumber).IsRequired();
        builder.Property(order => order.Scope).IsRequired();
        builder.Property(order => order.InitialDrawingRevisionId).IsRequired();
        builder.Property(order => order.CurrentDrawingRevisionId).IsRequired();
        builder.Property(order => order.Status).IsRequired();
        builder.Property(order => order.CreatedByUserId).IsRequired();
        builder.Property(order => order.CreatedAt).IsRequired();
        builder.Property(order => order.UpdatedByUserId).IsRequired();
        builder.Property(order => order.UpdatedAt).IsRequired();

        builder.HasIndex(order => new { order.ProjectId, order.OrderNumber }).IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(order => order.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<DrawingRevision>()
            .WithMany()
            .HasForeignKey(order => order.InitialDrawingRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<DrawingRevision>()
            .WithMany()
            .HasForeignKey(order => order.CurrentDrawingRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
