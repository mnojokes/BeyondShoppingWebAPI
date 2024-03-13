using BeyondShopping.Contracts.Objects;
using FluentValidation;

namespace BeyondShopping.Application.Validators;

public class ItemValidator : AbstractValidator<OrderItem>
{
    public ItemValidator()
    {
        RuleFor(item => item.Id).SetValidator(new IdValidator());

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
