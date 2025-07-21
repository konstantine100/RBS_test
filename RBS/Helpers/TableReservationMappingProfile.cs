using AutoMapper;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class TableReservationMappingProfile : Profile
{
    public TableReservationMappingProfile()
    {
        CreateMap<AddReservation, TableReservation>()
            .ForMember(dest => dest.BookingDateEnd, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<TableReservation, TableReservationDTO>().ReverseMap();
        CreateMap<TableReservation, OverallReservations>().ReverseMap();
    }
}