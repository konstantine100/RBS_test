using AutoMapper;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class ChairReservationMappingProfile : Profile
{
    public ChairReservationMappingProfile()
    {
        CreateMap<AddReservation, ChairReservation>()
            .ForMember(dest => dest.BookingDateEnd, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<ChairReservation, ChairReservationDTO>().ReverseMap();
        CreateMap<ChairReservation, OverallReservations>().ReverseMap();
    }
}