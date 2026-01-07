using bidify_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NotificationId)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(255); 

            builder.Property(x => x.IsRead)
                   .HasDefaultValue(false);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(x => x.ReadAt)
                   .IsRequired(false);

            builder.HasIndex(x => new { x.UserId, x.IsRead, x.IsDeleted });

            builder.HasOne(x => x.User)
                   .WithMany(x => x.UserNotifications)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
