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
        CreateMap<AddReservation, ChairReservation>().ReverseMap();
        CreateMap<ChairReservation, ChairReservationDTO>().ReverseMap();
        CreateMap<ChairReservation, OverallReservations>().ReverseMap();
    }
}