using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<Events, AddEvent>().ReverseMap();
        CreateMap<Events, EventDTO>().ReverseMap();
    }
}