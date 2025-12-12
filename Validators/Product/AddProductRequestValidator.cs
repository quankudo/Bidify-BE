using FluentValidation;
using bidify_be.DTOs.Product;

namespace bidify_be.Validators.Product
{
    public class AddProductRequestValidator : AbstractValidator<AddProductRequest>
    {
        public AddProductRequestValidator()
        {
            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            // Description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            // CategoryId
            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty).WithMessage("CategoryId is required.");

            // Thumbnail
            RuleFor(x => x.Thumbnail)
                .NotEmpty().WithMessage("Thumbnail image is required.");

            // Brand (optional)
            RuleFor(x => x.Brand)
                .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Brand));

            RuleFor(x => x.Images)
            .NotEmpty().WithMessage("At least 1 image is required.");

            // Prevent duplicate attribute keys
            RuleFor(x => x.Attributes)
                .Must(attrs => attrs.Select(a => a.Key.ToLower()).Distinct().Count() == attrs.Count)
                .WithMessage("Attribute keys must be unique.");

            // Prevent duplicate tag ids
            RuleFor(x => x.Tags)
                .Must(tags => tags.Select(t => t.TagId).Distinct().Count() == tags.Count)
                .WithMessage("Tags must not contain duplicate TagIds.");

            // Images
            RuleForEach(x => x.Images).SetValidator(new ProductImageRequestValidator());

            // Attributes
            RuleForEach(x => x.Attributes).SetValidator(new ProductAttributeRequestValidator());

            // Tags
            RuleForEach(x => x.Tags).SetValidator(new ProductTagRequestValidator());
        }
    }

    // Image validator
    public class ProductImageRequestValidator : AbstractValidator<ProductImageRequest>
    {
        public ProductImageRequestValidator()
        {
            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.");

            RuleFor(x => x.PublicId)
                .NotEmpty().WithMessage("PublicId is required.");
        }
    }

    // Attribute validator
    public class ProductAttributeRequestValidator : AbstractValidator<ProductAttributeRequest>
    {
        public ProductAttributeRequestValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Attribute key is required.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Attribute value is required.");
        }
    }

    // Tag validator
    public class ProductTagRequestValidator : AbstractValidator<ProductTagRequest>
    {
        public ProductTagRequestValidator()
        {
            RuleFor(x => x.TagId)
                .NotEqual(Guid.Empty).WithMessage("TagId is required.");
        }
    }
}
