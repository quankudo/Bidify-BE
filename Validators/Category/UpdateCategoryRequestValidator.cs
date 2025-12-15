using bidify_be.DTOs.Category;
using FluentValidation;

namespace bidify_be.Validators.Category
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator() {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.PublicId)
                .NotEmpty().WithMessage("PublicId is required.")
                .MaximumLength(200).WithMessage("PublicId must not exceed 200 characters.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .MaximumLength(500).WithMessage("ImageUrl must not exceed 500 characters.")
                .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
                    .WithMessage("ImageUrl must be a valid absolute URL.");
        }
    }
}
