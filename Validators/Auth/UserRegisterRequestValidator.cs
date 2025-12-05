using bidify_be.DTOs.Auth;
using FluentValidation;

namespace bidify_be.Validators.Auth
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MinimumLength(3).WithMessage("UserName must be at least 3 characters")
                .MaximumLength(50).WithMessage("UserName cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.ReferredBy)
                .MaximumLength(20).WithMessage("Referral code cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.ReferredBy));
        }
    }
}
