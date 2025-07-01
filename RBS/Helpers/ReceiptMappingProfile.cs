using AutoMapper;
using RBS.DTOs;
using RBS.Models;

namespace RBS.Helpers
{
    public class ReceiptMappingProfile : Profile
    {
        public ReceiptMappingProfile()
        {
            CreateMap<ReceiptDTO, Receipt>().ReverseMap();
        }
    }
}