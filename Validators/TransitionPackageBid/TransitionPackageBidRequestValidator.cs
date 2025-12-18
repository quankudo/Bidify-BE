using FluentValidation;
using bidify_be.DTOs.TransitionPackageBid;

namespace bidify_be.Validators.TransitionPackageBid
{
    public class TransitionPackageBidRequestValidator : AbstractValidator<TransitionPackageBidRequest>
    {
        public TransitionPackageBidRequestValidator()
        {
            RuleFor(x => x.PackageBidId)
                .NotEmpty().WithMessage("PackageBidId is required.")
                .Must(id => id != Guid.Empty).WithMessage("PackageBidId cannot be an empty GUID.");
        }
    }
}
