using bidify_be.DTOs.Category;
using FluentValidation;

namespace bidify_be.Validators.Category
{
    public class AddCategoryRequestValidator : AbstractValidator<AddCategoryRequest>
    {
        public AddCategoryRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.")
                .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
                    .WithMessage("ImageUrl must be a valid absolute URL.");
        }
    }
}
