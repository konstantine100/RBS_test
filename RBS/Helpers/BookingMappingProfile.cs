using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<AddReservation, Booking>().ReverseMap();
        CreateMap<Booking, BookingDTO>().ReverseMap();
    }
}