using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class FoodCategoryMappingProfile : Profile
{
    public FoodCategoryMappingProfile()
    {
        CreateMap<FoodCategory, AddFoodCategory>().ReverseMap();
        CreateMap<FoodCategory, FoodCategoryDTO>().ReverseMap();
    }
}