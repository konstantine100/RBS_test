using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class FoodMappingProfile : Profile
{
    public FoodMappingProfile()
    {
        CreateMap<Food, AddFood>().ReverseMap();
        CreateMap<Food, FoodDTO>().ReverseMap();
    }
}