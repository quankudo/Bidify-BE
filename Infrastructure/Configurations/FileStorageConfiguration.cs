using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bidify_be.Infrastructure.Configurations
{
    public class FileStorageConfiguration : IEntityTypeConfiguration<FileStorage>
    {
        public void Configure(EntityTypeBuilder<FileStorage> builder)
        {
            // Primary key
            builder.HasKey(x => x.PublicId);

            // PublicId
            builder.Property(x => x.PublicId)
                   .HasMaxLength(255)
                   .IsRequired();

            // Enum -> string (TEMP / USED)
            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .HasDefaultValue(FileStatus.Temp)
                   .IsRequired();

            // CreatedAt
            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            // Index phục vụ cleanup job
            builder.HasIndex(x => new { x.Status, x.CreatedAt });
        }
    }
}
