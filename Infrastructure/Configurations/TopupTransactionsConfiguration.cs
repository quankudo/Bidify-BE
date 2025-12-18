using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Persistence.Configurations
{
    public class TopupTransactionConfiguration
        : IEntityTypeConfiguration<TopupTransaction>
    {
        public void Configure(EntityTypeBuilder<TopupTransaction> builder)
        {
            builder.ToTable("topup_transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.PaymentMethod)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.TransactionCode)
                   .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(x => x.ClientOrderId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.RequestPayload)
                   .IsRequired()
                   .HasColumnType("longtext");

            builder.Property(x => x.ResponsePayload)
                   .IsRequired(false)
                   .HasColumnType("longtext");

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.TransactionCode).IsUnique();
            builder.HasIndex(x => x.ClientOrderId).IsUnique();
        }
    }
}
