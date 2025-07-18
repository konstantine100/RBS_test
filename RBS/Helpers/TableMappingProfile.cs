using AutoMapper;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Helpers;

public class TableMappingProfile : Profile
{
    public TableMappingProfile()
    {
        CreateMap<AddTable, Table>().ReverseMap();
        CreateMap<Table, TableDTO>().ReverseMap();
        CreateMap<Table, LayoutTableDTO>().ReverseMap();
    }
}