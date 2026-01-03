using bidify_be.Domain.Enums;
using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.Product
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
        public ProductCondition Condition { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string ThumbnailPublicId { get; set; } = string.Empty;
        public string? Note { get; set; }

        public ICollection<ProductImageResponse> Images { get; set; } = new List<ProductImageResponse>();
        public ICollection<ProductAttributeResponse> Attributes { get; set; } = new List<ProductAttributeResponse>();
        public ICollection<ProductTagResponse> ProductTags { get; set; } = new List<ProductTagResponse>();
    }

    public class ProductShortResponse //hiển thị ở trang quản lý sản phẩm của mỗi user
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
        public ProductCondition Condition { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class ProductShortResponseForList //hiển thị ở trang quản lý sản phẩm của mỗi user
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
    }

    public class ProductForTableResponse //hiển thị ở table trang admin
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
        public ProductCondition Condition { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string? Note { get; set; }
        public UserShortResponse User {  get; set; }
    }

    public class ProductImageResponse
    {
        public Guid Id { get; set; } 
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
    }

    public class ProductAttributeResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ProductTagResponse
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
    }
}
