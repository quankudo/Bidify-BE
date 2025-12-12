using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Product
{
    public class AddProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }
        public ProductCondition Condition { get; set; }
        public string Thumbnail { get; set; } = string.Empty;

        public ICollection<ProductImageRequest> Images { get; set; } = new List<ProductImageRequest>();
        public ICollection<ProductAttributeRequest> Attributes { get; set; } = new List<ProductAttributeRequest>();
        public ICollection<ProductTagRequest> Tags { get; set; } = new List<ProductTagRequest>();
    }

    public class ProductImageRequest {
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
    }

    public class ProductAttributeRequest {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ProductTagRequest
    {
        public Guid TagId { get; set; }
    }
}
