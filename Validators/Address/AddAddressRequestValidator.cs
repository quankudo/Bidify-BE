using bidify_be.DTOs.Address;
using FluentValidation;

namespace bidify_be.Validators.Address
{
    public class AddAddressRequestValidator : AbstractValidator<AddAddressRequest>
    {
        public AddAddressRequestValidator() {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x=>x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(100).WithMessage("UserName must not exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is required.")
                .MaximumLength(20).WithMessage("PhoneNumber must not exceed 20 characters.");

            RuleFor(x => x.LineOne)
                .NotEmpty().WithMessage("LineOne is required.")
                .MaximumLength(255).WithMessage("LineOne must not exceed 255 characters.");

            RuleFor(x => x.LineTwo)
                .NotEmpty().WithMessage("LineTwo is required.")
                .MaximumLength(255).WithMessage("LineTwo must not exceed 255 characters.");

            RuleFor(x => x.IsDefault)
                .NotNull().WithMessage("IsDefault is required.");
        }
    }
}
