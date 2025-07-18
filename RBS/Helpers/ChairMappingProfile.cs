using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class ChairMappingProfile : Profile
{
    public ChairMappingProfile()
    {
        CreateMap<AddChair, Chair>().ReverseMap();
        CreateMap<Chair, ChairDTO>().ReverseMap();
    }
}