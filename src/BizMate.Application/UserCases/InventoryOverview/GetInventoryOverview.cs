using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Enums;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.InventoryOverview
{
    public sealed class GetInventoryOverviewRequest : IRequest<InventoryOverviewResponse>
    {
        public Guid? CategoryGroupId { get; set; }
        public string? Keyword { get; set; }
        public bool IncludeInactive { get; set; }
    }

    public sealed class GetInventoryOverviewHandler
        : IRequestHandler<GetInventoryOverviewRequest, InventoryOverviewResponse>
    {
        private const string UngroupedGroupKey = "ungrouped";
        private const string UngroupedGroupName = "Chưa phân nhóm";
        private const string UncategorizedCategoryKey = "uncategorized";
        private const string UncategorizedCategoryName = "Chưa phân loại";

        private readonly IInventoryOverviewRepository _repository;
        private readonly IUserSession _userSession;
        private readonly ILogger<GetInventoryOverviewHandler> _logger;

        public GetInventoryOverviewHandler(
            IInventoryOverviewRepository repository,
            IUserSession userSession,
            ILogger<GetInventoryOverviewHandler> logger)
        {
            _repository = repository;
            _userSession = userSession;
            _logger = logger;
        }

        public async Task<InventoryOverviewResponse> Handle(
            GetInventoryOverviewRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var keyword = request.Keyword?.Trim();

                if (request.CategoryGroupId.HasValue)
                {
                    var exists = await _repository.CategoryGroupExistsAsync(
                        storeId,
                        request.CategoryGroupId.Value,
                        request.IncludeInactive,
                        cancellationToken);

                    if (!exists)
                    {
                        return new InventoryOverviewResponse(
                            false,
                            "Nhóm loại sản phẩm không tồn tại hoặc bạn không có quyền xem dữ liệu này.");
                    }
                }

                var rows = await _repository.GetOverviewRowsAsync(
                    storeId,
                    request.CategoryGroupId,
                    keyword,
                    request.IncludeInactive,
                    cancellationToken);

                return new InventoryOverviewResponse(BuildOverview(rows));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể tải tổng quan tồn kho.");
                return new InventoryOverviewResponse(
                    false,
                    "Không thể tải tổng quan tồn kho. Vui lòng thử lại.");
            }
        }

        private static InventoryOverviewDto BuildOverview(List<InventoryOverviewRowDto> rows)
        {
            var groups = rows
                .GroupBy(row => new
                {
                    Id = row.CategoryGroupId,
                    Key = row.CategoryGroupId?.ToString() ?? UngroupedGroupKey,
                    Name = string.IsNullOrWhiteSpace(row.CategoryGroupName)
                        ? UngroupedGroupName
                        : row.CategoryGroupName!,
                    CreatedDate = row.CategoryGroupCreatedDate
                })
                .OrderBy(group => group.Key.CreatedDate.HasValue ? 0 : 1)
                .ThenBy(group => group.Key.CreatedDate)
                .ThenBy(group => group.Key.Name)
                .ThenBy(group => group.Key.Id?.ToString() ?? UngroupedGroupKey)
                .Select(group =>
                {
                    var categories = group
                        .GroupBy(row => new
                        {
                            Id = row.ProductCategoryId,
                            Key = row.ProductCategoryId?.ToString() ?? UncategorizedCategoryKey,
                            Name = string.IsNullOrWhiteSpace(row.ProductCategoryName)
                                ? UncategorizedCategoryName
                                : row.ProductCategoryName!,
                            CreatedDate = row.ProductCategoryCreatedDate
                        })
                        .OrderBy(category => category.Key.CreatedDate.HasValue ? 0 : 1)
                        .ThenBy(category => category.Key.CreatedDate)
                        .ThenBy(category => category.Key.Name)
                        .ThenBy(category => category.Key.Id?.ToString() ?? UncategorizedCategoryKey)
                        .Select(category =>
                        {
                            var products = category
                                .OrderBy(row => row.ProductName)
                                .ThenBy(row => row.ProductCode)
                                .ThenBy(row => row.ProductId)
                                .Select(row => new InventoryOverviewProductDto
                                {
                                    ProductId = row.ProductId,
                                    ProductCode = row.ProductCode,
                                    ProductName = row.ProductName,
                                    Unit = row.Unit,
                                    UnitName = GetUnitName(row.Unit),
                                    ProductCategoryId = row.ProductCategoryId,
                                    ProductCategoryName = category.Key.Name,
                                    CategoryGroupId = group.Key.Id,
                                    CategoryGroupName = group.Key.Name,
                                    IsSerialTracked = row.IsSerialTracked,
                                    StockQuantity = row.StockQuantity,
                                    ReservedQuantity = row.ReservedQuantity,
                                    AvailableQuantity = row.AvailableQuantity,
                                    StockStatus = row.AvailableQuantity <= 0 ? "OUT_OF_STOCK" : "IN_STOCK",
                                    UpdatedDate = row.UpdatedDate
                                })
                                .ToList();

                            return new InventoryOverviewProductCategoryDto
                            {
                                CategoryKey = category.Key.Key,
                                ProductCategoryId = category.Key.Id,
                                ProductCategoryName = category.Key.Name,
                                CategoryGroupId = group.Key.Id,
                                CategoryGroupName = group.Key.Name,
                                CreatedDate = category.Key.CreatedDate,
                                ProductCount = products.Count,
                                TotalStockQuantity = products.Sum(x => x.StockQuantity),
                                TotalReservedQuantity = products.Sum(x => x.ReservedQuantity),
                                TotalAvailableQuantity = products.Sum(x => x.AvailableQuantity),
                                OutOfStockProductCount = products.Count(x => x.AvailableQuantity <= 0),
                                Products = products
                            };
                        })
                        .Where(category => category.ProductCount > 0)
                        .ToList();

                    return new InventoryOverviewCategoryGroupDto
                    {
                        CategoryGroupKey = group.Key.Key,
                        CategoryGroupId = group.Key.Id,
                        CategoryGroupName = group.Key.Name,
                        CreatedDate = group.Key.CreatedDate,
                        ProductCount = categories.Sum(x => x.ProductCount),
                        TotalStockQuantity = categories.Sum(x => x.TotalStockQuantity),
                        TotalReservedQuantity = categories.Sum(x => x.TotalReservedQuantity),
                        TotalAvailableQuantity = categories.Sum(x => x.TotalAvailableQuantity),
                        OutOfStockProductCount = categories.Sum(x => x.OutOfStockProductCount),
                        Categories = categories
                    };
                })
                .Where(group => group.ProductCount > 0)
                .ToList();

            return new InventoryOverviewDto
            {
                TotalProducts = groups.Sum(x => x.ProductCount),
                TotalStockQuantity = groups.Sum(x => x.TotalStockQuantity),
                TotalReservedQuantity = groups.Sum(x => x.TotalReservedQuantity),
                TotalAvailableQuantity = groups.Sum(x => x.TotalAvailableQuantity),
                OutOfStockProductCount = groups.Sum(x => x.OutOfStockProductCount),
                CategoryGroups = groups
            };
        }

        private static string GetUnitName(int unit)
        {
            return unit switch
            {
                (int)ProductUnit.Piece => "Cái",
                (int)ProductUnit.Box => "Hộp",
                (int)ProductUnit.Carton => "Thùng",
                (int)ProductUnit.Kilogram => "Kg",
                (int)ProductUnit.Liter => "Lít",
                (int)ProductUnit.Tree => "Cây",
                (int)ProductUnit.Meter => "Met",
                _ => "Khác"
            };
        }
    }
}
