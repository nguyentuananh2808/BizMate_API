using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt
{
    public class UpdateImportReceiptRequest : IRequest<UpdateImportReceiptResponse>
    {
        [Required]
        public Guid Id { get; set; }
        public string? SupplierName { get; set; }
        public string? DeliveryAddress { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string? Description { get; set; }
        [Required]
        public Guid RowVersion { get; set; }
        public ICollection<UpdateImportReceiptDetail> Details { get; set; } = new List<UpdateImportReceiptDetail>();
    }
    public class UpdateImportReceiptDetail
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
