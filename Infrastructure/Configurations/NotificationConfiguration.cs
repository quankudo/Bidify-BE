using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NotificationType)
                   .IsRequired();

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.RelatedAuctionId)
                   .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasIndex(x => x.RelatedAuctionId);

            builder.HasMany(x => x.UserNotifications)
                   .WithOne(x => x.Notification)
                   .HasForeignKey(x => x.NotificationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
