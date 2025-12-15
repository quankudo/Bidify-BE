using bidify_be.DTOs.Users;
using FluentValidation;

namespace bidify_be.Validators.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required")
                .MaximumLength(50).WithMessage("User name must not exceed 50 characters");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender must be Male, Female, or Other");

            RuleFor(x => x.Avatar)
                .Must(a => string.IsNullOrEmpty(a) || Uri.IsWellFormedUriString(a, UriKind.Absolute))
                .WithMessage("Avatar must be a valid URL");
        }
    }
}
