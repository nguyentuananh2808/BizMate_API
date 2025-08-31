using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class StatusMappingProfile : Profile
    {
        public StatusMappingProfile()
        {
            CreateMap<Status, StatuseCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group));
        }
    }
}
