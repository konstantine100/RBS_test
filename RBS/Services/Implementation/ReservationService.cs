using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class ReservationService : IReservationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public ReservationService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ApiResponse<List<OverallReservations>>> MyReservations(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<List<OverallReservations>>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            List<OverallReservations> allReservations = new List<OverallReservations>();
            
            var spaceReservations = await _context.SpaceReservations
                .Include(x => x.Space)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            var tableReservations = await _context.TableReservations
                .Include(x => x.Table)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            var chairReservations = await _context.ChairReservations
                .Include(x => x.Chair)
                .Where(x => x.UserId == userId)
                .ToListAsync();
            
            allReservations.AddRange(spaceReservations.Select(space => _mapper.Map<OverallReservations>(space)).ToList());
            allReservations.AddRange(tableReservations.Select(table => _mapper.Map<OverallReservations>(table)).ToList());
            allReservations.AddRange(chairReservations.Select(chair => _mapper.Map<OverallReservations>(chair)).ToList());
            
            var response = ApiResponseService<List<OverallReservations>>
                .Response200(allReservations);
            return response;
        }
    }
    
}