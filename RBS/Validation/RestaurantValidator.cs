using System.Data;
using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class RestaurantValidator : AbstractValidator<Restaurant>
{
    public RestaurantValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(2, 50).WithMessage("Title must be between 2 and 50 characters.");
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .Length(2, 50).WithMessage("Address must be between 2 and 50 characters.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^(\+995)?\s?(5\d{8})$").WithMessage("Phone number must be a valid Georgian mobile number (e.g., +995 5XXXXXXXX).");
        RuleFor(x => x.Lat)
            .NotNull().WithMessage("Latitude is required.")
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Lon)
            .NotNull().WithMessage("Longitude is required.")
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}