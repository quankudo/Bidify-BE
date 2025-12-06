using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                   .IsRequired();

            builder.Property(a => a.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(a => a.LineOne)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(a => a.LineTwo)
                   .HasMaxLength(255);

            builder.Property(a => a.IsDefault)
                   .HasDefaultValue(false);
        }
    }
}
