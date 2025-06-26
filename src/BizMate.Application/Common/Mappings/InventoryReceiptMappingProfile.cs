using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using BizMate.Domain.Entities;
using BizMate.Public.Extensions;

namespace BizMate.Application.Common.Mappings
{
    public class InventoryReceiptMappingProfile : Profile
    {
        public InventoryReceiptMappingProfile()
        {
            CreateMap<Dto.CoreDto.InventoryReceiptDetailDto,
                UserCases.InventoryReceipt.Commands.CreateInventoryReceipt.InventoryReceiptDetailDto>();

            CreateMap<InventoryReceipt, InventoryReceiptCoreDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.InventoryCode, opt => opt.MapFrom(src => src.InventoryCode))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<InventoryReceipt, CreateInventoryReceiptResponse>().IgnoreAllMembers()
               .ForMember(dest => dest.InventoryCode, opt => opt.MapFrom(src => src.InventoryCode))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
               .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName))
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
               .ForMember(dest => dest.InventoryDetails, opt => opt.MapFrom(src => src.Details));

            CreateMap<InventoryReceiptDetail, Dto.CoreDto.InventoryReceiptDetailDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit));
        }
    }
}
