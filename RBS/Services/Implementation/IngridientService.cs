using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implementation;

public class IngridientService : IIngridientService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public IngridientService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<IngredientDTO>> AddIngredient(int foodId, AddIngredient request)
    {
        var food = await _context.Foods
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.Id == foodId);

        if (food == null)
        {
            var response = ApiResponseService<IngredientDTO>
                .Response(null, "food not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var ingredient = _mapper.Map<Ingredient>(request);
            var validator = new IngredientValidator();
            var result = validator.Validate(ingredient);

            if (!result.IsValid)
            {
                string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                var response = ApiResponseService<IngredientDTO>
                    .Response(null, errors, StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                if (food.Ingredients.Any(x => x.EnglishName.ToLower() == ingredient.EnglishName.ToLower()) ||
                    food.Ingredients.Any(x => x.GeorgianName.ToLower() == ingredient.GeorgianName.ToLower()))
                {
                    var response = ApiResponseService<IngredientDTO>
                        .Response(null, "ingredient already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    ingredient.FoodId = foodId;
                    food.Ingredients.Add(ingredient);
                    await _context.SaveChangesAsync();
                
                    var response = ApiResponseService<IngredientDTO>
                        .Response200(_mapper.Map<IngredientDTO>(ingredient));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<IngredientDTO>> UpdateIngredient(int ingridientId, bool isEnglish, string changeTo)
    {
        var ingredient = await _context.Ingredients
            .Include(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == ingridientId);

        if (ingredient == null)
        {
            var response = ApiResponseService<IngredientDTO>
                .Response(null, "ingredient not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (isEnglish)
            {
                if (ingredient.Food.Ingredients.Any(x => x.EnglishName.ToLower() == changeTo.ToLower()))
                {
                    var response = ApiResponseService<IngredientDTO>
                        .Response(null, "ingredient already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    ingredient.EnglishName = changeTo;
                    var validator = new IngredientValidator();
                    var result = validator.Validate(ingredient);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                        var response = ApiResponseService<IngredientDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        var response = ApiResponseService<IngredientDTO>
                            .Response200(_mapper.Map<IngredientDTO>(ingredient));
                        return response;
                    }
                }
            }
            else
            {
                if (ingredient.Food.Ingredients.Any(x => x.GeorgianName.ToLower() == changeTo.ToLower()))
                {
                    var response = ApiResponseService<IngredientDTO>
                        .Response(null, "ingredient already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    ingredient.GeorgianName = changeTo;
                    var validator = new IngredientValidator();
                    var result = validator.Validate(ingredient);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                        var response = ApiResponseService<IngredientDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        var response = ApiResponseService<IngredientDTO>
                            .Response200(_mapper.Map<IngredientDTO>(ingredient));
                        return response;
                    }
                }
            }
            
        }
    }

    public async Task<ApiResponse<IngredientDTO>> DeleteIngredient(int ingridientId)
    {
        var ingredient = await _context.Ingredients
            .Include(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == ingridientId);

        if (ingredient == null)
        {
            var response = ApiResponseService<IngredientDTO>
                .Response(null, "ingredient not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<IngredientDTO>
                .Response200(_mapper.Map<IngredientDTO>(ingredient));
            return response;
        }
    }
}