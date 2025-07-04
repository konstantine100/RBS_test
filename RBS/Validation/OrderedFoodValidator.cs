using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class OrderedFoodValidator : AbstractValidator<OrderedFood>
{
    public OrderedFoodValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity cannot be empty")
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThan(1000).WithMessage("Quantity must be less than 1000");
        RuleFor(x => x.MessageToStuff)
            .MaximumLength(1000).WithMessage("MessageToStuff must be between 1000 characters");
    }
}