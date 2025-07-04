using FluentValidation;
using RBS.Models;

namespace RBS.Validation;

public class FoodCategoryValidator : AbstractValidator<FoodCategory>
{
    public FoodCategoryValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name cannot be empty")
            .Length(2, 50).WithMessage("Category name must be between 2 and 50 characters");
    }
}