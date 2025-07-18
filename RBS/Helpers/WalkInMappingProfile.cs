using AutoMapper;
using RBS.DTOs;
using RBS.Models;

namespace RBS.Helpers;

public class WalkInMappingProfile : Profile
{
    public WalkInMappingProfile()
    {
        CreateMap<WalkIn, WalkInDTO>().ReverseMap();
    }
}