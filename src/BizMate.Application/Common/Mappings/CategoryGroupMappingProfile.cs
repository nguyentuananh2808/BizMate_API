using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class CategoryGroupMappingProfile : Profile
    {
        public CategoryGroupMappingProfile()
        {
            CreateMap<CategoryGroup, CategoryGroupCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ProductCategoryCount, opt => opt.MapFrom(src =>
                    src.ProductCategories.Count(c => !c.IsDeleted)))
                .ForMember(dest => dest.ProductCategoryIds, opt => opt.MapFrom(src =>
                    src.ProductCategories.Where(c => !c.IsDeleted).Select(c => c.Id).ToList()))
                .ForMember(dest => dest.ProductCategories, opt => opt.MapFrom(src =>
                    src.ProductCategories.Where(c => !c.IsDeleted).ToList()));
        }
    }
}
