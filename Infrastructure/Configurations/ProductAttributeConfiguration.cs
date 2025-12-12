using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            builder.HasKey(pa => pa.Id);

            // Unique: một product chỉ có một attribute với Key nhất định
            builder.HasIndex(pa => new { pa.ProductId, pa.Key })
                   .IsUnique();

            builder.Property(pa => pa.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pa => pa.Value)
                .IsRequired()
                .HasMaxLength(200);
        }
    }

}
