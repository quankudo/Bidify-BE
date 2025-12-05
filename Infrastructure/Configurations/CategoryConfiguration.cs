using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Title)
                   .IsUnique();

            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            // ImageUrl (optional, nhưng nên hạn chế độ dài)
            builder.Property(c => c.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(c => c.Status)
                   .HasDefaultValue(true);
        }
    }
}
