using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class AdminService : IAdminService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public AdminService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<UserDTO>> MakeUserHost(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId && x.Role == ROLES.User);

        if (user == null)
        {
            var response = ApiResponseService<UserDTO>
                .Response(null, "user not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            user.Role = ROLES.Host;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<UserDTO>
                .Response200(_mapper.Map<UserDTO>(user));
            return response;
        }
    }

    public async Task<ApiResponse<List<UserDTO>>> SeeHosts(int restaurantId)
    {
        var restaurant = await _context.Restaurants
            .Include(x => x.Hosts)
            .FirstOrDefaultAsync(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<List<UserDTO>>
                .Response(null, "restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<List<UserDTO>>
                .Response200(_mapper.Map<List<UserDTO>>(restaurant.Hosts));
            return response;
        }
    }

    public async Task<ApiResponse<List<WalkIn>>> SeeHostWalkIns(int restaurantId, int hostId)
    {
        var walkIns = await _context.WalkIns
            .Include(x => x.Host)
            .Include(x => x.Table)
            .Include(x => x.Chair)
            .Where(x => x.Id == hostId && x.Host.RestaurantId == restaurantId)
            .ToListAsync();

        if (!walkIns.Any())
        {
            var response = ApiResponseService<List<WalkIn>>
                .Response(null, "Walk ins not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var response = ApiResponseService<List<WalkIn>>
                .Response200(_mapper.Map<List<WalkIn>>(walkIns));
            return response;
        }
    }

    public async Task<ApiResponse<UserDTO>> DemoteHost(int restaurantId, int hostId)
    {
        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == hostId && x.Role == ROLES.Host && x.RestaurantId == restaurantId);

        if (host == null)
        {
            var response = ApiResponseService<UserDTO>
                .Response(null, "host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(x => x.Id == restaurantId);
            
            restaurant.Hosts.Remove(host);
            host.Role = ROLES.User;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<UserDTO>
                .Response200(_mapper.Map<UserDTO>(host));
            return response;
        }
    }
}