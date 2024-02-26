using BeyondShopping.Contracts.Requests;
using FluentValidation;

namespace BeyondShopping.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(request => request.UserId)
            .SetValidator(new IdValidator());

        RuleFor(request => request.Items.Count)
            .GreaterThan(0).WithMessage("Item list cannot be empty.");

        RuleForEach(request => request.Items)
            .SetValidator(new ItemValidator());
    }
}
