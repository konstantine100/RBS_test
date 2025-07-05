using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class FoodCategoryValidator : AbstractValidator<FoodCategory>
{
    public FoodCategoryValidator()
    {
        RuleFor(ingredient => ingredient.CategoryEnglishName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        RuleFor(ingredient => ingredient.CategoryGeorgianName)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
    }
}