using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class IngredientMappingProfile : Profile
{
    public IngredientMappingProfile()
    {
        CreateMap<Ingredient, AddIngredient>().ReverseMap();
        CreateMap<Ingredient, IngredientDTO>().ReverseMap();
    }
}