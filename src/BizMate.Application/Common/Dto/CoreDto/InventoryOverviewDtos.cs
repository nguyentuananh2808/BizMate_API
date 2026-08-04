using BizMate.Application.Common.Responses;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public sealed class InventoryOverviewDto
    {
        public int TotalProducts { get; set; }
        public int TotalStockQuantity { get; set; }
        public int TotalReservedQuantity { get; set; }
        public int TotalAvailableQuantity { get; set; }
        public int OutOfStockProductCount { get; set; }
        public List<InventoryOverviewCategoryGroupDto> CategoryGroups { get; set; } = new();
    }

    public sealed class InventoryOverviewCategoryGroupDto
    {
        public string CategoryGroupKey { get; set; } = string.Empty;
        public Guid? CategoryGroupId { get; set; }
        public string CategoryGroupName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public int ProductCount { get; set; }
        public int TotalStockQuantity { get; set; }
        public int TotalReservedQuantity { get; set; }
        public int TotalAvailableQuantity { get; set; }
        public int OutOfStockProductCount { get; set; }
        public List<InventoryOverviewProductCategoryDto> Categories { get; set; } = new();
    }

    public sealed class InventoryOverviewProductCategoryDto
    {
        public string CategoryKey { get; set; } = string.Empty;
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public Guid? CategoryGroupId { get; set; }
        public string CategoryGroupName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public int ProductCount { get; set; }
        public int TotalStockQuantity { get; set; }
        public int TotalReservedQuantity { get; set; }
        public int TotalAvailableQuantity { get; set; }
        public int OutOfStockProductCount { get; set; }
        public List<InventoryOverviewProductDto> Products { get; set; } = new();
    }

    public sealed class InventoryOverviewProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Unit { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public Guid? CategoryGroupId { get; set; }
        public string? CategoryGroupName { get; set; }
        public bool IsSerialTracked { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }

    public sealed class InventoryOverviewRowDto
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Unit { get; set; }
        public bool IsSerialTracked { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string? ProductCategoryName { get; set; }
        public DateTime? ProductCategoryCreatedDate { get; set; }
        public Guid? CategoryGroupId { get; set; }
        public string? CategoryGroupName { get; set; }
        public DateTime? CategoryGroupCreatedDate { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public sealed class InventoryOverviewResponse : BaseResponse
    {
        public InventoryOverviewDto? Overview { get; }

        public InventoryOverviewResponse(InventoryOverviewDto overview, bool success = true, string? message = null)
            : base(success, message ?? string.Empty)
        {
            Overview = overview;
        }

        public InventoryOverviewResponse(bool success, string? message)
            : base(success, message ?? string.Empty)
        {
        }
    }
}
