using AutoMapper;
using RBS.DTOs;
using RBS.Models;

namespace RBS.Helpers;
public class ReceiptMappingProfile : Profile
{
    public ReceiptMappingProfile()
    {
        CreateMap<ReceiptDTO, Receipt>()
            .ForMember(dest => dest.CustomerDetails, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.CustomerDetails, opt => opt.MapFrom(src => src.CustomerDetails));
    }
}
