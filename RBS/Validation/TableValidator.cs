using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class TableValidator : AbstractValidator<Table>
{
    public TableValidator()
    {
        RuleFor(x=> x.TableNumber)
            .NotEmpty().WithMessage("Table Number is required")
            .Length(1,20).WithMessage("Table Number must be between 1 and 20 characters");
        RuleFor(x=> x.TablePrice)
            .NotEmpty().WithMessage("Table Price is required")
            .GreaterThan(0).WithMessage("Table Price must be greater than 0")
            .LessThanOrEqualTo(100000).WithMessage("Table Price must be less than 100000");
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