using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class RestaurantMappingProfile : Profile
{
    public RestaurantMappingProfile()
    {
        CreateMap<AddRestaurant, Restaurant>().ReverseMap();
        CreateMap<Restaurant, RestaurantDTO>().ReverseMap();
    }
}