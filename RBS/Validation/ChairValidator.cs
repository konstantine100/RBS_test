using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class ChairValidator : AbstractValidator<Chair>
{
    public ChairValidator()
    {
        RuleFor(x=> x.ChairNumber)
            .NotEmpty().WithMessage("Chair Number is required")
            .Length(1,20).WithMessage("Chair Number must be between 1 and 20 characters");
        RuleFor(x=> x.ChairPrice)
            .NotEmpty().WithMessage("Chair Price is required")
            .GreaterThan(0).WithMessage("Chair Price must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Chair Price must be less than 10000");
        RuleFor(x=> x.MinSpent)
            .NotEmpty().WithMessage("Min Price is required")
            .GreaterThan(0).WithMessage("Min Price must be greater than 0")
            .LessThanOrEqualTo(400).WithMessage("Min Price must be less than 400");
        RuleFor(x=> x.Xlocation)
            .NotEmpty().WithMessage("X location is required")
            .GreaterThanOrEqualTo(-1000).WithMessage("X location must be greater than -1001")
            .LessThanOrEqualTo(1000).WithMessage("X location must be less than 1001");
        RuleFor(x=> x.Ylocation)
            .NotEmpty().WithMessage("Y location is required")
            .GreaterThanOrEqualTo(-1000).WithMessage("Y location must be greater than -1001")
            .LessThanOrEqualTo(1000).WithMessage("Y location must be less than 1001");
    }
}