using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt
{
    public class GetImportReceiptResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid RowVersion { get; set; }
        public string? SupplierName { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusCode { get; set; }
        public ICollection<ImportReceiptDetailDto> Details { get; set; } = new List<ImportReceiptDetailDto>();
        public GetImportReceiptResponse() : base(true)
        {
        }
     public GetImportReceiptResponse(bool success = true, string? message = null) : base(success, message)
        {
        }
    }
    }
