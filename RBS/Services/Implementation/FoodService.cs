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

public class FoodService : IFoodService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public FoodService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<FoodDTO>> AddFood(int categoryId, AddFood request)
    {
        var foodCategory = await _context.FoodCategories
            .Include(x => x.Foods)
            .FirstOrDefaultAsync(x => x.Id == categoryId);

        if (foodCategory == null)
        {
            var response = ApiResponseService<FoodDTO>
                .Response(null, "Food Category not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var food = _mapper.Map<Food>(request);
            var validator = new FoodValidator();
            var result = validator.Validate(food);

            if (!result.IsValid)
            {
                string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                var response = ApiResponseService<FoodDTO>
                    .Response(null, errors, StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                if (foodCategory.Foods.Any(x => x.EnglishName.ToLower() == food.EnglishName.ToLower()) ||
                    foodCategory.Foods.Any(x => x.GeorgianName.ToLower() == food.GeorgianName.ToLower()))
                {
                    var response = ApiResponseService<FoodDTO>
                        .Response(null, "Food already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    food.FoodCategoryId = categoryId;
                    foodCategory.Foods.Add(food);
                    
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<FoodDTO>
                        .Response200(_mapper.Map<FoodDTO>(food));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<FoodDTO>> UpdateFood(int foodId, string changeParameter, string changeTo)
    {
        var food = await _context.Foods
            .Include(x => x.FoodCategory)
            .FirstOrDefaultAsync(x => x.Id == foodId);

        if (food == null)
        {
            var response = ApiResponseService<FoodDTO>
                .Response(null, "Food not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (changeParameter.ToLower() == "englishName")
            {
                if (food.FoodCategory.Foods.Any(x => x.EnglishName.ToLower() == changeTo.ToLower()))
                {
                    var response = ApiResponseService<FoodDTO>
                        .Response(null, "Food already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    food.EnglishName = changeTo;
                    var validator = new FoodValidator();
                    var result = validator.Validate(food);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                        var response = ApiResponseService<FoodDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<FoodDTO>
                            .Response200(_mapper.Map<FoodDTO>(food));
                        return response;
                    }
                }
            }
            else if (changeParameter.ToLower() == "georgianName")
            {
                if (food.FoodCategory.Foods.Any(x => x.GeorgianName.ToLower() == changeTo.ToLower()))
                {
                    var response = ApiResponseService<FoodDTO>
                        .Response(null, "Food already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    food.GeorgianName = changeTo;
                    var validator = new FoodValidator();
                    var result = validator.Validate(food);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                        var response = ApiResponseService<FoodDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<FoodDTO>
                            .Response200(_mapper.Map<FoodDTO>(food));
                        return response;
                    }
                }
            }
            else if (changeParameter.ToLower() == "price")
            {
                food.Price = int.Parse(changeTo);
                var validator = new FoodValidator();
                var result = validator.Validate(food);

                if (!result.IsValid)
                {
                    string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                    var response = ApiResponseService<FoodDTO>
                        .Response(null, errors, StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    await _context.SaveChangesAsync();
                        
                    var response = ApiResponseService<FoodDTO>
                        .Response200(_mapper.Map<FoodDTO>(food));
                    return response;
                }
            }
            else
            {
                var response = ApiResponseService<FoodDTO>
                    .Response(null, "Wrong change parameter", StatusCodes.Status400BadRequest);
                return response;
            }
        }
    }

    public async Task<ApiResponse<FoodDTO>> SeeFoodDetails(int foodId)
    {
        var food = await _context.Foods
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.Id == foodId);

        if (food == null)
        {
            var response = ApiResponseService<FoodDTO>
                .Response(null, "Food not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<FoodDTO>
                .Response200(_mapper.Map<FoodDTO>(food));
            return response;
        }
    }

    public async Task<ApiResponse<FoodDTO>> DeleteFood(int foodId)
    {
        var food = await _context.Foods
            .FirstOrDefaultAsync(x => x.Id == foodId);

        if (food == null)
        {
            var response = ApiResponseService<FoodDTO>
                .Response(null, "Food not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<FoodDTO>
                .Response200(_mapper.Map<FoodDTO>(food));
            return response;
        }
    }

    public async Task<ApiResponse<FoodDTO>> FoodAvailabilityChange(int foodId)
    {
        var food = await _context.Foods
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.Id == foodId);

        if (food == null)
        {
            var response = ApiResponseService<FoodDTO>
                .Response(null, "Food not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (food.IsAvailable)
            {
                food.IsAvailable = false;
                await _context.SaveChangesAsync();
            
                var response = ApiResponseService<FoodDTO>
                    .Response200(_mapper.Map<FoodDTO>(food));
                return response;
            }
            else
            {
                food.IsAvailable = true;
                await _context.SaveChangesAsync();
            
                var response = ApiResponseService<FoodDTO>
                    .Response200(_mapper.Map<FoodDTO>(food));
                return response;
            }
        }
    }
}