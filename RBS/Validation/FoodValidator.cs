using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class FoodValidator : AbstractValidator<Food>
{
    public FoodValidator()
    {
        RuleFor(ingredient => ingredient.EnglishName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        RuleFor(ingredient => ingredient.GeorgianName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price cannot be empty")
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(10000).WithMessage("Price must be less than 100900");
        
    }
}