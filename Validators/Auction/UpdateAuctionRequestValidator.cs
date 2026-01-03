using bidify_be.DTOs.Auction;
using FluentValidation;

namespace bidify_be.Validators.Auction
{
    public class UpdateAuctionRequestValidator : AbstractValidator<UpdateAuctionRequest>
    {
        public UpdateAuctionRequestValidator()
        {
            // ProductId
            RuleFor(x => x.ProductId)
                .NotEqual(Guid.Empty)
                .WithMessage("ProductId is required.");

            // StartAt
            RuleFor(x => x.StartAt)
                .NotEmpty()
                .WithMessage("StartAt is required.")
                .Must(startAt => startAt > DateTime.UtcNow)
                .WithMessage("StartAt must be in the future.");

            // EndAt
            RuleFor(x => x.EndAt)
                .NotEmpty()
                .WithMessage("EndAt is required.")
                .GreaterThan(x => x.StartAt)
                .WithMessage("EndAt must be greater than StartAt.");

            // StartPrice
            RuleFor(x => x.StartPrice)
                .GreaterThan(0)
                .WithMessage("StartPrice must be greater than 0.");

            // StepPrice
            RuleFor(x => x.StepPrice)
                .GreaterThan(0)
                .WithMessage("StepPrice must be greater than 0.");

            // Tags - not empty (optional, bạn có thể bỏ nếu không bắt buộc)
            RuleFor(x => x.Tags)
                .NotEmpty()
                .WithMessage("At least 1 tag is required.");

            // Prevent duplicate tag ids
            RuleFor(x => x.Tags)
                .Must(tags => tags.Select(t => t.TagId).Distinct().Count() == tags.Count)
                .WithMessage("Tags must not contain duplicate TagIds.");

            // Tag validator
            RuleForEach(x => x.Tags)
                .SetValidator(new AuctionTagRequestValidator());
        }
    }
}
