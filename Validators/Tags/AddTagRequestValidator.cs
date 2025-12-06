using bidify_be.DTOs.Tags;
using FluentValidation;

namespace bidify_be.Validators.Tags
{
    public class AddTagRequestValidator : AbstractValidator<AddTagRequest>
    {
        public AddTagRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be a valid TagType enum value.");
        }
    }
}
