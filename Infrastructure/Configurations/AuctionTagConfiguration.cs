using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class AuctionTagConfiguration : IEntityTypeConfiguration<AuctionTag>
    {
        public void Configure(EntityTypeBuilder<AuctionTag> builder)
        {
            builder.ToTable("AuctionTags");

            builder.HasKey(pt => new { pt.AuctionId, pt.TagId });

            builder
                .HasOne(pt => pt.Auction)
                .WithMany(p => p.AuctionTags)
                .HasForeignKey(pt => pt.AuctionId);

            builder
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.AuctionTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}
