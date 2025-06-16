using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty")
            .Length(2,50).WithMessage("First name must be between 2 and 50 characters");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .Length(2,50).WithMessage("Last name must be between 2 and 50 characters");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email address");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .Length(8,30).WithMessage("Password must be between 8 and 30 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
    }
}