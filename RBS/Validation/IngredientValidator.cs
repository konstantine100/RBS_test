using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class IngredientValidator : AbstractValidator<Ingredient>
{
    public IngredientValidator()
    {
        RuleFor(ingredient => ingredient.EnglishName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        RuleFor(ingredient => ingredient.GeorgianName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
    }
}