using FluentValidation;
using bidify_be.DTOs.Gift;

namespace bidify_be.Validators.Gift
{
    public class UpdateGiftRequestValidator : AbstractValidator<UpdateGiftRequest>
    {
        public UpdateGiftRequestValidator()
        {

            RuleFor(x => x.QuantityBid)
                .GreaterThan(0).WithMessage("QuantityBid must be greater than zero.");

            RuleFor(x => x.GiftTypeId)
                .NotEqual(Guid.Empty).WithMessage("GiftTypeId is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status is required.");
        }
    }
}
