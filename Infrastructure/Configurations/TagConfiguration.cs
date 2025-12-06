using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {

        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Title)
                   .IsUnique();

            builder.Property(c => c.Title)
               .HasMaxLength(100)
               .IsRequired();

            builder.Property(c => c.Status)
                   .HasDefaultValue(true);
        }
    }
}
