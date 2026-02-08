using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;

namespace bidify_be.Domain.Entities
{
    public class Address : EntityBase<Guid>, IDateTracking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public string LineOne { get; set; } = string.Empty;
        public string LineTwo { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
