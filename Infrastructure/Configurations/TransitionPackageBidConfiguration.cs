using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using bidify_be.Domain.Entities;

namespace bidify_be.Infrastructure.Persistence.Configurations
{
    public class TransitionPackageBidConfiguration : IEntityTypeConfiguration<TransitionPackageBid>
    {
        public void Configure(EntityTypeBuilder<TransitionPackageBid> builder)
        {
            // Table name
            builder.ToTable("TransitionPackageBids");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.UserId)
                   .IsRequired();

            builder.Property(t => t.PackageBidId)
                   .IsRequired();

            builder.Property(t => t.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.BidCount)
                   .IsRequired();


            builder.HasIndex(t => t.UserId); 
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.PackageBidId);
            builder.HasOne<PackageBid>() 
                   .WithMany()
                   .HasForeignKey(t => t.PackageBidId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
