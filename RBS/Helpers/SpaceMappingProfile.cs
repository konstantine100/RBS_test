using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class SpaceMappingProfile : Profile
{
    public SpaceMappingProfile()
    {
        CreateMap<AddSpace, Space>().ReverseMap();
        CreateMap<Space, SpaceDTO>().ReverseMap();
        CreateMap<Space, LayoutSpaceDTO>().ReverseMap();
    }
}