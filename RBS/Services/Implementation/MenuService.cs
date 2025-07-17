using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class MenuService : IMenuService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public MenuService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<MenuDTO>> AddMenu(int restaurantId)
    {
        var restaurant = await _context.Restaurants
            .Include(x => x.Menu)
            .FirstOrDefaultAsync(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<MenuDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (restaurant.Menu != null)
            {
                var response = ApiResponseService<MenuDTO>
                    .Response(null, "Menu already exists", StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                var menu = new Menu
                {
                    RestaurantId = restaurantId,
                    Name = $"{restaurant.Title}'s Menu"
                };
                restaurant.Menu = menu;
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<MenuDTO>
                    .Response200(_mapper.Map<MenuDTO>(menu));
                return response;
            }
        }
    }

    public async Task<ApiResponse<MenuDTO>> DeleteMenu(int menuId)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(x => x.Id == menuId);

        if (menu == null)
        {
            var response = ApiResponseService<MenuDTO>
                .Response(null, "Menu not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<MenuDTO>
                .Response200(_mapper.Map<MenuDTO>(menu));
            return response;
        }
    }

    public async Task<ApiResponse<MenuDTO>> SeeMenu(int restaurantId)
    {
        var menu = await _context.Menus
            .Include(x => x.Categories)
            .ThenInclude(x => x.Foods)
            .ThenInclude(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.RestaurantId == restaurantId);

        if (menu == null)
        {
            var response = ApiResponseService<MenuDTO>
                .Response(null, "Menu not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<MenuDTO>
                .Response200(_mapper.Map<MenuDTO>(menu));
            return response;
        }
    }

    public async Task<ApiResponse<List<FoodDTO>>> SearchFoodInMenu(int menuId, string searchTerm)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(x => x.RestaurantId == menuId);

        if (menu == null)
        {
            var response = ApiResponseService<List<FoodDTO>>
                .Response(null, "Menu not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var searchToLower = searchTerm.ToLower();
            var foods = await _context.Foods
                .Include(x => x.Ingredients)
                .Include(x => x.FoodCategory)
                .Where(x => x.FoodCategory.MenuId == menuId &&
                            (x.EnglishName.ToLower().Contains(searchToLower) ||
                             x.GeorgianName.ToLower().Contains(searchToLower) ||
                             x.Ingredients.Any(x => x.EnglishName.ToLower().Contains(searchToLower)) ||
                             x.Ingredients.Any(x => x.GeorgianName.ToLower().Contains(searchToLower)) ||
                             x.FoodCategory.CategoryEnglishName.ToLower().Contains(searchToLower)) ||
                             x.FoodCategory.CategoryGeorgianName.ToLower().Contains(searchToLower))
                .ToListAsync();
            
            var response = ApiResponseService<List<FoodDTO>>
                .Response200(_mapper.Map<List<FoodDTO>>(foods));
            return response;
        }
    }
}