using System.Diagnostics.Contracts;
using RBS.Models;
using RBS.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RBS.DTOs;
using Table = RBS.Models.Table;

namespace RBS.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AddUser, User>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<AddUser, UserDTO>().ReverseMap();

        CreateMap<AddRestaurant, Restaurant>().ReverseMap();
        CreateMap<Restaurant, RestaurantDTO>().ReverseMap();
        
        CreateMap<AddSpace, Space>().ReverseMap();
        CreateMap<Space, SpaceDTO>().ReverseMap();
        
        CreateMap<AddTable, Table>().ReverseMap();
        CreateMap<Table, TableDTO>().ReverseMap();
        
        CreateMap<AddChair, Chair>().ReverseMap();
        CreateMap<Chair, ChairDTO>().ReverseMap();
        
        CreateMap<AddReservation, Booking>().ReverseMap();
        CreateMap<Booking, BookingDTO>().ReverseMap();
        
        CreateMap<AddReservation, ReservationBooking>().ReverseMap();
        CreateMap<ReservationBooking, ReservationBookingDTO>().ReverseMap(); // not using 
        
        CreateMap<AddReservation, SpaceReservation>().ReverseMap();
        CreateMap<SpaceReservation, SpaceReservationDTO>().ReverseMap();
        
        CreateMap<AddReservation, TableReservation>().ReverseMap();
        CreateMap<TableReservation, TableReservationDTO>().ReverseMap();
        
        CreateMap<AddReservation, ChairReservation>().ReverseMap();
        CreateMap<ChairReservation, ChairReservationDTO>().ReverseMap();
        
    }
}