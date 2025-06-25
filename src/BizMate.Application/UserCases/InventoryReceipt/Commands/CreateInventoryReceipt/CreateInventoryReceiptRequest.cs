using BizMate.Domain.Entities;
using MediatR;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt
{
    public class CreateInventoryReceiptRequest : IRequest<CreateInventoryReceiptResponse>
    {

        /// <summary>
        /// "Import" hoặc "Export"
        /// </summary>
        public int Type { get; set; } = default!;

        // Thêm thông tin tĩnh tùy theo loại phiếu
        public string? SupplierName { get; set; }     // Cho phiếu nhập
        public string? CustomerName { get; set; }     // Cho phiếu xuất
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Description { get; set; }
        public List<InventoryReceiptDetailDto> Details { get; set; } = new();
    }
}
