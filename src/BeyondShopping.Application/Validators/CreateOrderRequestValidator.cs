using BeyondShopping.Contracts.Requests;
using FluentValidation;

namespace BeyondShopping.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0).WithMessage("UserId must be an integer greater than 0.");

        RuleFor(request => request.Items.Count)
            .GreaterThan(0).WithMessage("Item list cannot be empty.");

        RuleFor(request => request.Items)
            .Must(items => items.All(itemId => itemId > 0)).WithMessage("Item Ids must be integers greater than 0.");
    }
}
