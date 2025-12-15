using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class FileStorage
    {
        public string PublicId { get; set; } = null!;
        public FileStatus Status { get; set; } = FileStatus.Temp;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
