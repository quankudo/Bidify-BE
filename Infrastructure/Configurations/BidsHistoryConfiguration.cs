using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class BidsHistoryConfiguration : IEntityTypeConfiguration<BidsHistory>
    {
        public void Configure(EntityTypeBuilder<BidsHistory> builder)
        {
            builder.ToTable("BidsHistory");

            builder.HasKey(x => x.Id);

            // ---------- Properties ----------
            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.AuctionId)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // ---------- Relationships ----------
            builder.HasOne(x => x.User)
                .WithMany(u => u.BidsHistories) 
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // (OPTIONAL) nếu có Auction entity
            builder.HasOne<Auction>()
                .WithMany(a => a.BidsHistories)
                .HasForeignKey(x => x.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.AuctionId);
            builder.HasIndex(x => new { x.AuctionId, x.CreatedAt });
        }
    }
}
