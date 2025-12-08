using bidify_be.DTOs.Voucher;
using FluentValidation;

namespace bidify_be.Validators.Voucher
{
    public class UpdateVoucherRequestValidator : AbstractValidator<UpdateVoucherRequest>
    {
        public UpdateVoucherRequestValidator() {
            RuleFor(x => x.PackageBidId)
                .NotEmpty().WithMessage("PackageBidId is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

            RuleFor(x => x.VoucherTypeId)
                .NotEmpty().WithMessage("VoucherTypeId is required.");

            RuleFor(x => x.Discount)
                .GreaterThan(0).WithMessage("Discount must be greater than 0.");

            RuleFor(x => x.DiscountType)
                .IsInEnum().WithMessage("DiscountType is invalid.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("ExpiryDate must be in the future.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status is invalid.");
        }
    }
}
