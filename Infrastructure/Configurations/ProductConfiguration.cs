using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            // Name index
            builder.HasIndex(p => p.Name);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.Brand)
                .HasMaxLength(100);

            builder.Property(p => p.Thumbnail)
                .HasMaxLength(500);

            builder.Property(p => p.Note)
                .HasMaxLength(500);

            // Enum mapping (int)
            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Condition)
                .IsRequired();

            builder.HasOne<Category>()
               .WithMany()
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1 Product – N ProductImage
            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(img => img.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1 Product – N ProductAttribute
            builder.HasMany(p => p.Attributes)
                .WithOne()
                .HasForeignKey(attr => attr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1 Product – N ProductTag
            builder.HasMany(p => p.ProductTags)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
