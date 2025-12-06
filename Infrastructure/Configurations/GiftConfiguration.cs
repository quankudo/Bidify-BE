using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class GiftConfiguration : IEntityTypeConfiguration<Gift>
    {
        public void Configure(EntityTypeBuilder<Gift> builder)
        {
            builder.ToTable("Gifts");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Code)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(g => g.QuantityBid)
                     .IsRequired();

            builder.Property(g => g.Description)
                   .HasMaxLength(500);

            builder.Property(g => g.Status)
                   .HasDefaultValue(true);

            builder.Property(g => g.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(g => g.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<GiftType>()       
               .WithMany()              
               .HasForeignKey(g => g.GiftTypeId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
