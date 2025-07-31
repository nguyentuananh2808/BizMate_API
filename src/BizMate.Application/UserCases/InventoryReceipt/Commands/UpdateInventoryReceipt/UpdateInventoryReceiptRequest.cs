using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt
{
    public class UpdateInventoryReceiptRequest : IRequest<UpdateInventoryReceiptResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public byte[] RowVersion { get; set; }
        public string? SupplierName { get; set; }     // Cho phiếu nhập
        public string? CustomerName { get; set; }     // Cho phiếu xuất
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Description { get; set; }
        public List<InventoryReceiptDetailRequestDto> Details { get; set; } = new();
    }
}
