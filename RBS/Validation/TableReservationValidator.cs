using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class TableReservationValidator : AbstractValidator<TableReservation>
{
    public TableReservationValidator()
    {
        RuleFor(x => x.BookingDate)
            .NotEmpty().WithMessage("date cannot be empty")
            .GreaterThan(DateTime.UtcNow).WithMessage("invalid date");
    }
}