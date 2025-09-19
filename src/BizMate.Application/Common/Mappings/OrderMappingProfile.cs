using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderCoreDto>().IgnoreAllMembers()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
              .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
              .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
              .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
              .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
              .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
              .ForMember(dest => dest.CustomerType, opt => opt.MapFrom(src => src.CustomerType))
              .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
              .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name))
              .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


            CreateMap<OrderDetail, OrderDetailDto>().IgnoreAllMembers()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));



        }
    }
}
