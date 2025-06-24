using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(x => x.BookingDate)
            .NotEmpty().WithMessage("date cannot be empty")
            .GreaterThan(DateTime.UtcNow).WithMessage("invalid date");
        RuleFor(x => x.BookingDateEnd)
            .GreaterThan(x => x.BookingDate.AddHours(1)).WithMessage("invalid date");
    }
}