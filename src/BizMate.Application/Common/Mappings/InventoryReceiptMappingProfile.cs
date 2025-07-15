using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt;
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
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<InventoryReceipt, CreateInventoryReceiptResponse>().IgnoreAllMembers()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
               .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.InventoryCode, opt => opt.MapFrom(src => src.Code))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
               .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
               .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName))
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
               .ForMember(dest => dest.InventoryDetails, opt => opt.MapFrom(src => src.Details))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


            CreateMap<InventoryReceipt, GetInventoryReceiptResponse>().IgnoreAllMembers()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
               .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.InventoryCode, opt => opt.MapFrom(src => src.Code))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
               .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.SupplierName))
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
               .ForMember(dest => dest.InventoryDetails, opt => opt.MapFrom(src => src.Details))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


            CreateMap<InventoryReceiptDetail, Dto.CoreDto.InventoryReceiptDetailDto>().IgnoreAllMembers()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<InventoryReceiptDetail, BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt.InventoryReceiptDetailDto>();

        }
    }
}
