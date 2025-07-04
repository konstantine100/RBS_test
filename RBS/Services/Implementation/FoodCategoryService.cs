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

public class FoodCategoryService : IFoodCategoryService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public FoodCategoryService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<FoodCategoryDTO>> AddFoodCategory(int menuId, AddFoodCategory request)
    {
        var menu = await _context.Menus
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == menuId);

        if (menu == null)
        {
            var response = ApiResponseService<FoodCategoryDTO>
                .Response(null, "Food Category Not Found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var foodCategory = _mapper.Map<FoodCategory>(request);
            var validator = new FoodCategoryValidator();
            var result = validator.Validate(foodCategory);

            if (!result.IsValid)
            {
                string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                var response = ApiResponseService<FoodCategoryDTO>
                    .Response(null, errors, StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                if (menu.Categories.Any(x => x.CategoryName.ToLower() == foodCategory.CategoryName.ToLower()))
                {
                    var response = ApiResponseService<FoodCategoryDTO>
                        .Response(null, "category already exists", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    foodCategory.MenuId = menuId;
                    menu.Categories.Add(foodCategory);
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<FoodCategoryDTO>
                        .Response200(_mapper.Map<FoodCategoryDTO>(foodCategory));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<FoodCategoryDTO>> UpdateFoodCategory(int categoryId, string newCategoryName)
    {
        var foodCategory = await _context.FoodCategories
            .Include(x => x.MenuId)
            .FirstOrDefaultAsync(x => x.Id == categoryId);

        if (foodCategory == null)
        {
            var response = ApiResponseService<FoodCategoryDTO>
                .Response(null, "Food Category Not Found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (foodCategory.Menu.Categories.Any(x => x.CategoryName.ToLower() == foodCategory.CategoryName.ToLower()))
            {
                var response = ApiResponseService<FoodCategoryDTO>
                    .Response(null, "category already exists", StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                foodCategory.CategoryName = newCategoryName;
                var validator = new FoodCategoryValidator();
                var result = validator.Validate(foodCategory);

                if (!result.IsValid)
                {
                    string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

                    var response = ApiResponseService<FoodCategoryDTO>
                        .Response(null, errors, StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    await _context.SaveChangesAsync();
                    
                    var response = ApiResponseService<FoodCategoryDTO>
                        .Response200(_mapper.Map<FoodCategoryDTO>(foodCategory));
                    return response;
                }
            }
        }
    }

    public async Task<ApiResponse<FoodCategoryDTO>> SeeFoodCategory(int categoryId)
    {
        var foodCategory = await _context.FoodCategories
            .Include(x => x.Foods)
            .ThenInclude(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.Id == categoryId);

        if (foodCategory == null)
        {
            var response = ApiResponseService<FoodCategoryDTO>
                .Response(null, "food category not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<FoodCategoryDTO>
                .Response200(_mapper.Map<FoodCategoryDTO>(foodCategory));
            return response;
        }
    }

    public async Task<ApiResponse<FoodCategoryDTO>> DeleteFoodCategory(int categoryId)
    {
        var foodCategory = await _context.FoodCategories
            .Include(x => x.Foods)
            .ThenInclude(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.Id == categoryId);

        if (foodCategory == null)
        {
            var response = ApiResponseService<FoodCategoryDTO>
                .Response(null, "food category not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.FoodCategories.Remove(foodCategory);
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<FoodCategoryDTO>
                .Response200(_mapper.Map<FoodCategoryDTO>(foodCategory));
            return response;
        }
    }
}