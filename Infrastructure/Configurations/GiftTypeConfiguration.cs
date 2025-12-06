using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class GiftTypeConfiguration : IEntityTypeConfiguration<GiftType>
    {
        public void Configure(EntityTypeBuilder<GiftType> builder)
        {
            builder.ToTable("GiftTypes");

            builder.HasKey(gt => gt.Id);

            builder.Property(gt => gt.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(gt => gt.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(gt => gt.Description)
                   .HasMaxLength(500);

            builder.Property(gt => gt.Status)
                   .HasDefaultValue(true);

            builder.Property(gt => gt.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(gt => gt.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
