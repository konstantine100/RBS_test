using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class BookingValidator : AbstractValidator<ReservationBooking>
{
    public BookingValidator()
    {
        RuleFor(x => x.BookingDate)
            .NotEmpty().WithMessage("date cannot be empty")
            .GreaterThan(DateTime.UtcNow).WithMessage("invalid date");
        RuleFor(x => x.BookingDateEnd)
            .GreaterThanOrEqualTo(x => x.BookingDate.AddHours(2)).WithMessage("minimal time is 2 hours");
    }
}