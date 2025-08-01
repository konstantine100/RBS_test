using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implementation;

public class EventService : IEventService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public EventService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<ApiResponse<EventDTO>> CreateEvent(int adminId, AddEvent request)
    {
        var admin = await _context.Users
            .FirstOrDefaultAsync(x => x.Role == ROLES.Admin && x.Id == adminId);

        if (admin == null)
        {
            return ApiResponseService<EventDTO>
                .Response(null, "admin does not exist", StatusCodes.Status404NotFound);
        }
        else
        {
            var restaurant = await _context.Restaurants
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Id == admin.RestaurantId);
            
            var @event = _mapper.Map<Events>(request);
            var validator = new EventValidator();
            var result = validator.Validate(@event);

            if (!result.IsValid)
            {
                string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                return ApiResponseService<EventDTO>
                    .Response(null, $"errors: ${errors}", StatusCodes.Status400BadRequest);
            }
            else
            {
                @event.RestaurantId = restaurant.Id;
                restaurant.Events.Add(@event);
                await _context.SaveChangesAsync();
                
                return ApiResponseService<EventDTO>
                    .Response200(_mapper.Map<EventDTO>(@event));
            }
        }
    }

    public async Task<ApiResponse<List<EventDTO>>> GetActiveEvents(int restaurantId)
    {
        var events = await _context.Events
            .Where(x => x.EventDate.Day >= DateTime.UtcNow.Day)
            .OrderByDescending(x => x.PublishDate)
            .ToListAsync();
        
        return ApiResponseService<List<EventDTO>>
            .Response200(_mapper.Map<List<EventDTO>>(events));
    }

    public async Task<ApiResponse<List<EventDTO>>> GetPastEvents(int restaurantId)
    {
        var events = await _context.Events
            .Where(x => x.EventDate.Day < DateTime.UtcNow.Day)
            .OrderByDescending(x => x.PublishDate)
            .ToListAsync();
        
        return ApiResponseService<List<EventDTO>>
            .Response200(_mapper.Map<List<EventDTO>>(events));
    }

    public async Task<ApiResponse<List<EventDTO>>> SearchSortEvents(int restaurantId, string? searchString, string? sortBy)
    {
        var events = await _context.Events
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            events = events
                .Where(x => 
                    x.Name.ToLower().Contains(searchString.ToLower()) ||
                    x.Description.ToLower().Contains(searchString.ToLower()))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "name-asc":
                    events = events.OrderBy(x => x.Name).ToList();
                    break;
                case "name-desc":
                    events = events.OrderByDescending(x => x.Name).ToList();
                    break;
                case "publish-asc":
                    events = events.OrderBy(x => x.PublishDate).ToList();
                    break;
                case "publish-desc":
                    events = events.OrderByDescending(x => x.PublishDate).ToList();
                    break;
                case "date-asc":
                    events = events.OrderBy(x => x.EventDate).ToList();
                    break;
                case "date-desc":
                    events = events.OrderByDescending(x => x.EventDate).ToList();
                    break;
            }
        }
        
        return ApiResponseService<List<EventDTO>>
            .Response200(_mapper.Map<List<EventDTO>>(events));
    }

    public async Task<ApiResponse<EventDTO>> GetEventById(int eventId)
    {
        var @event = await _context.Events
            .FirstOrDefaultAsync(x => x.Id == eventId);
        
        return ApiResponseService<EventDTO>
            .Response200(_mapper.Map<EventDTO>(@event));
    }

    public async Task<ApiResponse<EventDTO>> UpdateEvent(int adminId, int eventId, string changeParameter, string changeTo)
    {
        var admin = await _context.Users
            .FirstOrDefaultAsync(x => x.Role == ROLES.Admin && x.Id == adminId);

        if (admin == null)
        {
            return ApiResponseService<EventDTO>
                .Response(null, "admin does not exist", StatusCodes.Status404NotFound);
        }
        else
        {
            var @event = await _context.Events
                .FirstOrDefaultAsync(x => x.Id == eventId && x.RestaurantId == admin.RestaurantId);

            if (@event == null)
            {
                return ApiResponseService<EventDTO>
                    .Response(null, "event does not exist", StatusCodes.Status404NotFound);
            }
            else
            {
                if (changeParameter == "name")
                {
                    @event.Name = changeTo;
                    var validator = new EventValidator();
                    var result = validator.Validate(@event);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        return ApiResponseService<EventDTO>
                            .Response(null, $"errors: {errors}", StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        return ApiResponseService<EventDTO>
                            .Response200(_mapper.Map<EventDTO>(@event));
                    }
                }
                else if (changeParameter == "description")
                {
                    @event.Description = changeTo;
                    var validator = new EventValidator();
                    var result = validator.Validate(@event);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        return ApiResponseService<EventDTO>
                            .Response(null, $"errors: {errors}", StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        return ApiResponseService<EventDTO>
                            .Response200(_mapper.Map<EventDTO>(@event));
                    }
                }
                else if (changeParameter == "image")
                {
                    @event.ImageUrl = changeTo;
                    var validator = new EventValidator();
                    var result = validator.Validate(@event);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        return ApiResponseService<EventDTO>
                            .Response(null, $"errors: {errors}", StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        return ApiResponseService<EventDTO>
                            .Response200(_mapper.Map<EventDTO>(@event));
                    }
                }
                else if (changeParameter == "date")
                {
                    @event.EventDate = DateTime.Parse(changeTo);
                    var validator = new EventValidator();
                    var result = validator.Validate(@event);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        return ApiResponseService<EventDTO>
                            .Response(null, $"errors: {errors}", StatusCodes.Status400BadRequest);
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        return ApiResponseService<EventDTO>
                            .Response200(_mapper.Map<EventDTO>(@event));
                    }
                }
                else
                {
                    return ApiResponseService<EventDTO>
                        .Response(null, "wrong change parameter", StatusCodes.Status400BadRequest);
                }
            }
        }
    }

    public async Task<ApiResponse<EventDTO>> DeleteEvent(int adminId, int eventId)
    {
        var admin = await _context.Users
            .FirstOrDefaultAsync(x => x.Role == ROLES.Admin && x.Id == adminId);

        if (admin == null)
        {
            return ApiResponseService<EventDTO>
                .Response(null, "admin does not exist", StatusCodes.Status404NotFound);
        }
        else
        {
            var @event = await _context.Events
                .FirstOrDefaultAsync(x => x.Id == eventId && x.RestaurantId == admin.RestaurantId);

            if (@event == null)
            {
                return ApiResponseService<EventDTO>
                    .Response(null, "event does not exist", StatusCodes.Status404NotFound);
            }
            else
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                
                return ApiResponseService<EventDTO>
                    .Response200(_mapper.Map<EventDTO>(@event));
            }
        }
    }
}