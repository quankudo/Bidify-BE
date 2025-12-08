using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.ToTable("Vouchers");

            // Khóa chính
            builder.HasKey(v => v.Id);

            // Thuộc tính
            builder.Property(v => v.Code)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(v => v.Description)
                   .HasMaxLength(200);

            builder.Property(v => v.Discount)
                   .HasColumnType("decimal(18,2)");

            // Enum mapping
            builder.Property(v => v.Status)
                   .HasConversion<int>();

            builder.Property(v => v.DiscountType)
                   .HasConversion<int>();

            // Quan hệ với GiftType
            builder.HasOne<GiftType>()
               .WithMany()
               .HasForeignKey(g => g.VoucherTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PackageBid>()
               .WithMany()
               .HasForeignKey(v => v.PackageBidId)
               .OnDelete(DeleteBehavior.Restrict);
            }
    }
}
