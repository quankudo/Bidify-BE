using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Persistence.Configurations
{
    public class WalletTransactionConfiguration
        : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            // Table
            builder.ToTable("wallet_transactions");

            // Primary key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(x => x.BalanceBefore)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.BalanceAfter)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ReferenceId)
                   .IsRequired(false);

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(255);

            // Indexes (query cực nhiều)
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.ReferenceId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
