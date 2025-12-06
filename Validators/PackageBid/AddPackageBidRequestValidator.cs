using bidify_be.DTOs.PackageBid;
using FluentValidation;

namespace bidify_be.Validators.PackageBid
{
    public class AddPackageBidRequestValidator : AbstractValidator<AddPackageBidRequest>
    {
        public AddPackageBidRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.BidQuantity)
                .GreaterThan(0).WithMessage("Bid quantity must be at least 1.");

            RuleFor(x => x.BgColor)
                .NotEmpty().WithMessage("Background color is required.")
                .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
                .WithMessage("BgColor must be a valid hex color code.");
        }
    }
}
