using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Domain.Entities.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.Addresses)
                   .WithOne()      
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(u => u.Balance)
                   .HasPrecision(18, 2);

            builder.Property(u => u.RateStar)
                   .HasPrecision(3, 2);

            builder.Property(u => u.Balance)
                   .HasDefaultValue(0);

            builder.Property(u => u.RateStar)
                   .HasDefaultValue(5);
        }
    }
}
