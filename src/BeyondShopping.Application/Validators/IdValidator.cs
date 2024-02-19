using FluentValidation;

namespace BeyondShopping.Application.Validators;

public class IdValidator : AbstractValidator<int>
{
    public IdValidator()
    {
        RuleFor(id => id)
            .GreaterThan(0).WithMessage("Id must be an integer greater than 0.");
    }
}
