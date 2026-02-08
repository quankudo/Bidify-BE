using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class FileStorage : EntityBase<Guid>
    {
        public string PublicId { get; set; } = null!;
        public FileStatus Status { get; set; } = FileStatus.Temp;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
