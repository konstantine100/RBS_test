using AutoMapper;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class SpaceReservationMappingProfile : Profile
{
    public SpaceReservationMappingProfile()
    {
        CreateMap<AddReservation, SpaceReservation>().ReverseMap();
        CreateMap<SpaceReservation, SpaceReservationDTO>().ReverseMap();
        CreateMap<SpaceReservation, OverallReservations>().ReverseMap();
    }
}