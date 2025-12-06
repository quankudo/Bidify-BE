using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class PackageBidConfiguration : IEntityTypeConfiguration<PackageBid>
    {
        public void Configure(EntityTypeBuilder<PackageBid> builder)
        {
            builder.ToTable("PackageBids");

            builder.HasKey(pb => pb.Id);

            builder.Property(pb => pb.Price)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(pb => pb.BidQuantity)
                   .IsRequired();

            builder.Property(pb => pb.BgColor)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(pb => pb.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(pb => pb.status)
                   .HasDefaultValue(true);
        }
    }
}
