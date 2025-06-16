using System.Diagnostics.Contracts;
using RBS.Models;
using RBS.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RBS.DTOs;

namespace RBS.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AddUser, User>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<AddUser, UserDTO>().ReverseMap();
        
    }
}