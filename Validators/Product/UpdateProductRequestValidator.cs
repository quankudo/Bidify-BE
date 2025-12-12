using bidify_be.DTOs.Product;
using FluentValidation;

namespace bidify_be.Validators.Product
{
    
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Thumbnail)
                .NotEmpty();

            RuleFor(x => x.Brand)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Brand));

            // Check duplicate attribute keys
            RuleFor(x => x.Attributes)
                .Must(attrs => attrs.Select(a => a.Key.ToLower()).Distinct().Count() == attrs.Count)
                .WithMessage("Attribute keys must be unique.");

            // Check duplicate tag ids
            RuleFor(x => x.Tags)
                .Must(tags => tags.Select(t => t.TagId).Distinct().Count() == tags.Count)
                .WithMessage("Tags must not contain duplicate TagIds.");

            RuleForEach(x => x.Images).SetValidator(new ProductImageRequestValidator());
            RuleForEach(x => x.Attributes).SetValidator(new ProductAttributeRequestValidator());
            RuleForEach(x => x.Tags).SetValidator(new ProductTagRequestValidator());
        }
    }

}
