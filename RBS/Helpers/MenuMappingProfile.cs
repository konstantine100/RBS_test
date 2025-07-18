using AutoMapper;
using RBS.DTOs;
using RBS.Models;

namespace RBS.Helpers;

public class MenuMappingProfile : Profile
{
    public MenuMappingProfile()
    {
        CreateMap<Menu, MenuDTO>().ReverseMap();
    }
}