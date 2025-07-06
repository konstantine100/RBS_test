using System.Diagnostics.Contracts;
using RBS.Models;
using RBS.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RBS.CORE;
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
        
        CreateMap<AddReservation, SpaceReservation>().ReverseMap();
        CreateMap<SpaceReservation, SpaceReservationDTO>().ReverseMap();
        
        CreateMap<AddReservation, TableReservation>().ReverseMap();
        CreateMap<TableReservation, TableReservationDTO>().ReverseMap();
        
        CreateMap<AddReservation, ChairReservation>().ReverseMap();
        CreateMap<ChairReservation, ChairReservationDTO>().ReverseMap();

        CreateMap<SpaceReservation, OverallReservations>().ReverseMap();
        CreateMap<TableReservation, OverallReservations>().ReverseMap();
        CreateMap<ChairReservation, OverallReservations>().ReverseMap();
        
        CreateMap<Menu, MenuDTO>().ReverseMap();
        
        CreateMap<FoodCategory, AddFoodCategory>().ReverseMap();
        CreateMap<FoodCategory, FoodCategoryDTO>().ReverseMap();
        
        CreateMap<Food, AddFood>().ReverseMap();
        CreateMap<Food, FoodDTO>().ReverseMap();
        
        CreateMap<Ingredient, AddIngredient>().ReverseMap();
        CreateMap<Ingredient, IngredientDTO>().ReverseMap();
        
        CreateMap<OrderedFood, AddOrderedFood>().ReverseMap();
        CreateMap<OrderedFood, OrderedFoodDTO>().ReverseMap();

        CreateMap<WalkIn, WalkInDTO>().ReverseMap();

        CreateMap<Space, LayoutSpaceDTO>().ReverseMap();
        CreateMap<Table, LayoutTableDTO>().ReverseMap();
    }
}