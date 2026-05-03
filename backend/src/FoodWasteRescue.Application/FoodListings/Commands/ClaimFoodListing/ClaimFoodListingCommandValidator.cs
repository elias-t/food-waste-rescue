using FluentValidation;

namespace FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;

public class ClaimFoodListingCommandValidator : AbstractValidator<ClaimFoodListingCommand>
{
    public ClaimFoodListingCommandValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes is not null);
    }
}
