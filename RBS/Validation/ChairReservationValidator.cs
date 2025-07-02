using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class ChairReservationValidator : AbstractValidator<ChairReservation>
{
    public ChairReservationValidator()
    {
        RuleFor(x => x.BookingDate)
            .NotEmpty().WithMessage("date cannot be empty")
            .GreaterThan(DateTime.UtcNow).WithMessage("invalid date");
    }
}