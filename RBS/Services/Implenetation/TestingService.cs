using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class TestingService
{
    // for testing
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public TestingService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    public ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        var validator = new RestaurantValidator();
        var result = validator.Validate(restaurant);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<RestaurantDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            
            var response = ApiResponseService<RestaurantDTO>
                .Response(_mapper.Map<RestaurantDTO>(restaurant), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<SpaceDTO> AddSpace(Guid restaurantId, AddSpace request)
    {
        var restaurant = _context.Restaurants
            .Include(x => x.Spaces)
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        var space = _mapper.Map<Space>(request);
        var validator = new SpaceValidator();
        var result = validator.Validate(space);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<SpaceDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            restaurant.Spaces.Add(space);
            _context.SaveChanges();
            
            var response = ApiResponseService<SpaceDTO>
                .Response(_mapper.Map<SpaceDTO>(space), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<TableDTO> AddTable(Guid spaceId, AddTable request)
    {
        var space = _context.Spaces
            .Include(x => x.Tables)
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        var table = _mapper.Map<Table>(request);
        var validator = new TableValidator();
        var result = validator.Validate(table);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<TableDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            space.Tables.Add(table);
            _context.SaveChanges();
            
            var response = ApiResponseService<TableDTO>
                .Response(_mapper.Map<TableDTO>(table), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<ChairDTO> AddChair(Guid tableId, AddChair request)
    {
        var table = _context.Tables
            .Include(x => x.Chairs)
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        var chair = _mapper.Map<Chair>(request);
        var validator = new ChairValidator();
        var result = validator.Validate(chair);

        if (!result.IsValid)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));

            var response = ApiResponseService<ChairDTO>
                .Response(null, errors, StatusCodes.Status400BadRequest);
            return response;
        }
        else
        {
            table.Chairs.Add(chair);
            _context.SaveChanges();
            
            var response = ApiResponseService<ChairDTO>
                .Response(_mapper.Map<ChairDTO>(chair), null, StatusCodes.Status200OK);
            return response;
        }
    }

    public ApiResponse<RestaurantDTO> DeleteRestaurant(Guid restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);

        if (restaurant == null)
        {
            var response = ApiResponseService<RestaurantDTO>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Restaurants.Remove(restaurant);
            _context.SaveChanges();

            var response = ApiResponseService<RestaurantDTO>
                .Response200(_mapper.Map<RestaurantDTO>(restaurant));
            return response;
        }
    }

    public ApiResponse<SpaceDTO> DeleteSpace(Guid spaceId)
    {
        var space = _context.Spaces
            .FirstOrDefault(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<SpaceDTO>
                .Response(null, "Space not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Spaces.Remove(space);
            _context.SaveChanges();

            var response = ApiResponseService<SpaceDTO>
                .Response200(_mapper.Map<SpaceDTO>(space));
            return response;
        }
    }

    public ApiResponse<TableDTO> DeleteTable(Guid tableId)
    {
        var table = _context.Tables
            .FirstOrDefault(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Tables.Remove(table);
            _context.SaveChanges();

            var response = ApiResponseService<TableDTO>
                .Response200(_mapper.Map<TableDTO>(table));
            return response;
        }
    }

    public ApiResponse<ChairDTO> DeleteChair(Guid chairId)
    {
        var chair = _context.Chairs
            .FirstOrDefault(x => x.Id == chairId);

        if (chair == null)
        {
            var response = ApiResponseService<ChairDTO>
                .Response(null, "Chair not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            _context.Chairs.Remove(chair);
            _context.SaveChanges();

            var response = ApiResponseService<ChairDTO>
                .Response200(_mapper.Map<ChairDTO>(chair));
            return response;
        }
    }

}