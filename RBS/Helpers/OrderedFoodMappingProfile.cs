using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class OrderedFoodMappingProfile : Profile
{
    public OrderedFoodMappingProfile()
    {
        CreateMap<OrderedFood, AddOrderedFood>().ReverseMap();
        CreateMap<OrderedFood, OrderedFoodDTO>().ReverseMap();
    }
}