using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.ToTable("Auctions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.ProductId)
                   .IsRequired();

            builder.Property(x => x.BidCount)
                   .HasDefaultValue(0);

            builder.Property(x => x.StartPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.StepPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.BuyNowPrice)
                   .HasPrecision(18, 2)
                   .IsRequired(false);

            builder.Property(x => x.StartAt)
                   .IsRequired();

            builder.Property(x => x.EndAt)
                   .IsRequired();

            builder.Property(x => x.Note)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(x => x.WinnerId)
                   .HasMaxLength(255)
                   .IsRequired(false);

            // ===================== RELATIONSHIPS ==============
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.AuctionTags)
                   .WithOne(x => x.Auction)
                   .HasForeignKey(x => x.AuctionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Status)
                   .IsRequired();

            // ===================== INDEXES ====================
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.EndAt);
            builder.HasIndex(x => x.Status);
        }
    }
}
