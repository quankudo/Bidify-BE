using bidify_be.Domain.Enums;
using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.Product
{
    // ================= BASE =================
    public abstract class ProductBaseResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
    }

    public abstract class ProductInfoResponse : ProductBaseResponse
    {
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }

        public ProductStatus Status { get; set; }
        public ProductCondition Condition { get; set; }

        public string? Note { get; set; }
    }

    // ================= DETAIL =================
    public class ProductResponse : ProductInfoResponse
    {
        public string CategoryName { get; set; } = string.Empty;

        public string ThumbnailPublicId { get; set; } = string.Empty;

        public ICollection<ProductImageResponse> Images { get; set; } = new List<ProductImageResponse>();
        public ICollection<ProductAttributeResponse> Attributes { get; set; } = new List<ProductAttributeResponse>();
        public ICollection<ProductTagResponse> ProductTags { get; set; } = new List<ProductTagResponse>();
    }

    // ================= SHORT =================
    public class ProductShortResponse : ProductInfoResponse
    {
        // dùng cho trang quản lý sản phẩm user
    }

    public class ProductShortForOrderResponse : ProductBaseResponse
    {
        // chỉ cần Id, Name, Thumbnail
    }

    public class ProductShortResponseForList : ProductBaseResponse
    {
        // dùng cho list đơn giản
    }

    // ================= ADMIN TABLE =================
    public class ProductForTableResponse : ProductInfoResponse
    {
        public UserShortResponse User { get; set; }
    }

    // ================= CHILD DTO =================
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
