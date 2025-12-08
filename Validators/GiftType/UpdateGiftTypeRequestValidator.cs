using FluentValidation;
using bidify_be.DTOs.GiftType;

namespace bidify_be.Validators.GiftType
{
    public class UpdateGiftTypeRequestValidator : AbstractValidator<UpdateGiftTypeRequest>
    {
        public UpdateGiftTypeRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(15).WithMessage("Code cannot exceed 15 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status is required.");
        }
    }
}
