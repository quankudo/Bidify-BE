using System.ComponentModel.DataAnnotations.Schema;

namespace bidify_be.Domain.Entities
{
    public class PackageBid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Price { get; set; }
        public int BidQuantity { get; set; }
        public string BgColor { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
