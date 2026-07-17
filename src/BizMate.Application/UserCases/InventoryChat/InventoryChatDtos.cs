using BizMate.Application.Common.Responses;
using MediatR;

namespace BizMate.Application.UserCases.InventoryChat
{
    public class AskInventoryChatRequest : IRequest<InventoryChatResponse>
    {
        public string Question { get; set; } = string.Empty;
    }

    public class InventoryChatResponse : BaseResponse
    {
        public string Intent { get; set; } = InventoryChatIntent.Unknown.ToString();
        public string Answer { get; set; } = string.Empty;
        public InventoryChatTableDto? Table { get; set; }
        public IReadOnlyList<string> Suggestions { get; set; } = Array.Empty<string>();

        public InventoryChatResponse(bool success = true, string? message = null)
            : base(success, message)
        {
        }
    }

    public class InventoryChatTableDto
    {
        public string Title { get; set; } = string.Empty;
        public IReadOnlyList<InventoryChatColumnDto> Columns { get; set; } = Array.Empty<InventoryChatColumnDto>();
        public IReadOnlyList<Dictionary<string, string>> Rows { get; set; } = Array.Empty<Dictionary<string, string>>();
    }

    public class InventoryChatColumnDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class ParsedInventoryQuestion
    {
        public InventoryChatIntent Intent { get; set; } = InventoryChatIntent.Unknown;
        public string RawQuestion { get; set; } = string.Empty;
        public string? Keyword { get; set; }
        public int? Threshold { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
        public string? DateLabel { get; set; }
    }

    public enum InventoryChatIntent
    {
        Unknown = 0,
        Help = 1,
        CheckStock = 2,
        SearchProduct = 3,
        LowStock = 4,
        TechnicianHoldings = 5,
        ImportByDate = 6,
        ExportByDate = 7,
        ProductHistory = 8
    }

    public class InventoryChatProductStockDto
    {
        public Guid ProductId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }
        public int Available { get; set; }
        public bool IsSerialTracked { get; set; }
    }

    public class InventoryChatHoldingDto
    {
        public string TechnicianName { get; set; } = string.Empty;
        public string? TechnicianPhone { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BorrowType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime LastBorrowedAt { get; set; }
    }

    public class InventoryChatReceiptDto
    {
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? PartnerName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductsSummary { get; set; } = string.Empty;
    }

    public class InventoryChatHistoryDto
    {
        public DateTime Date { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? ReferenceCode { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Actor { get; set; }
        public string? Note { get; set; }
    }
}
