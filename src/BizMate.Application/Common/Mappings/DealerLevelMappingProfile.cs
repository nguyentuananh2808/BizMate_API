using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class DealerLevelMappingProfile : Profile
    {
        public DealerLevelMappingProfile()
        {
            CreateMap<DealerLevel, DealerLevelCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));


            CreateMap<DealerLevel, DealerLevelDetailCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.DealerPriceForDealerLevel, opt => opt.MapFrom(src =>
                            src.DealerPrices.Select(dp => new DealerPriceForDealerLevelDetailCoreDto
                            {
                                ProductId = dp.ProductId,
                                NameProduct = dp.Product.Name,
                                UnitProduct = dp.Product.Unit,
                                Price = dp.Price,
                                DealerPriceId = dp.Id,
                                RowVersionDealerPrice = dp.RowVersion
                            })));

        }
    }
}
