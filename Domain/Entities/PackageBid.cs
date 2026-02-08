using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace bidify_be.Domain.Entities
{
    public class PackageBid : EntityBase<Guid>, IDateTracking
    {
        public decimal Price { get; set; }
        public int BidQuantity { get; set; }
        public string BgColor { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool status { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
