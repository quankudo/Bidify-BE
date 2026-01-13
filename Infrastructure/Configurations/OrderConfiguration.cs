using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.FinalPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<int>() 
                   .IsRequired();

            builder.Property(o => o.ReceiverName)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(o => o.ReceiverPhone)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(o => o.ShippingAddress)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(o => o.CancelReason)
                   .HasMaxLength(255);

            builder.Property(o => o.PaidAt)
                   .IsRequired(false);

            builder.Property(o => o.CreatedAt)
                   .IsRequired();

            builder.Property(o => o.UpdatedAt)
                   .IsRequired();

            // Order - Seller
            builder.HasOne(o => o.Seller)
                   .WithMany()
                   .HasForeignKey(o => o.SellerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order - Winner
            builder.HasOne(o => o.Winner)
                   .WithMany() 
                   .HasForeignKey(o => o.WinnerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order - Auction (1-1)
            builder.HasOne(o => o.Auction)
                   .WithOne()
                   .HasForeignKey<Order>(o => o.AuctionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(o => o.AuctionId).IsUnique();
            builder.HasIndex(o => o.SellerId);
            builder.HasIndex(o => o.WinnerId);
            builder.HasIndex(o => o.Status);
        }
    }
}
