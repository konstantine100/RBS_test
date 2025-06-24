using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class SpaceValidator : AbstractValidator<Space>
{
    public SpaceValidator()
    {
        RuleFor(x => x.SpacePrice)
            .NotEmpty().WithMessage("Space Price is required.")
            .GreaterThan(0).WithMessage("Space Price must be greater than 0.")
            .LessThan(10000000).WithMessage("Space Price must be less than 10000000.");
    }
}