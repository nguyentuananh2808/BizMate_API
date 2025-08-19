using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class DealerPriceMappingProfile : Profile
    {
        public DealerPriceMappingProfile()
        {
            CreateMap<DealerPrice, DealerPriceCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DealerLevelId, opt => opt.MapFrom(src => src.DealerLevelId))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));

        }
    }
}