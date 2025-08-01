using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class EventValidator : AbstractValidator<Events>
{
    public EventValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Maximum length of name is 200 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Maximum length of description is 2000 characters.");
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image is required.");
        RuleFor(x => x.EventDate)
            .NotNull().WithMessage("Date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Date must be in the future.");
    }
}