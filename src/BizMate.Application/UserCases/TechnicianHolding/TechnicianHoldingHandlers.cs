using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.TechnicianHolding
{
    public class BorrowingMutationResponse : BaseResponse
    {
        public BorrowingMutationResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class ExportOrderForTechnicianRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid OrderId { get; set; }
        public Guid? TechnicianId { get; set; }
        public List<ExportOrderForTechnicianItem> Items { get; set; } = new();
        public List<TechnicianExportRequest> TechnicianExports { get; set; } = new();
    }

    public class TechnicianExportRequest
    {
        public Guid TechnicianId { get; set; }
        public List<ExportOrderForTechnicianItem> Items { get; set; } = new();
    }

    public class ExportOrderForTechnicianItem
    {
        public Guid ProductId { get; set; }
        public int OrderedQuantity { get; set; }
        public int BorrowedQuantity { get; set; }
    }

    public class ReturnTechnicianHoldingRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid TechnicianId { get; set; }
        public List<ReturnTechnicianHoldingItem> Items { get; set; } = new();
    }

    public class ReturnTechnicianHoldingItem
    {
        public Guid ProductId { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Assigned;
        public int Quantity { get; set; }
    }

    public class UseBorrowedItemRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid OrderId { get; set; }
        public Guid? TechnicianId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateTechnicianBorrowRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid TechnicianId { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Daily;
        public DateOnly NeededDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public string? Description { get; set; }
        public List<TechnicianBorrowRequestItemInput> Items { get; set; } = new();
    }

    public class TechnicianBorrowRequestItemInput
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class GetTechnicianBorrowRequestsRequest : IRequest<GetTechnicianBorrowRequestsResponse>
    {
        public TechnicianBorrowRequestStatus? Status { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    public class GetTechnicianBorrowRequestsResponse : BaseResponse
    {
        public List<TechnicianBorrowRequestDto> Requests { get; set; } = new();

        public GetTechnicianBorrowRequestsResponse(List<TechnicianBorrowRequestDto> requests)
            : base(true)
        {
            Requests = requests;
        }

        public GetTechnicianBorrowRequestsResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class TechnicianBorrowRequestDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public Guid TechnicianId { get; set; }
        public string TechnicianName { get; set; } = default!;
        public string? Phone { get; set; }
        public TechnicianBorrowType BorrowType { get; set; }
        public string BorrowTypeName { get; set; } = default!;
        public TechnicianBorrowRequestStatus RequestStatus { get; set; }
        public string RequestStatusName { get; set; } = default!;
        public DateOnly NeededDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Description { get; set; }
        public string? RejectionReason { get; set; }
        public int TotalQuantity { get; set; }
        public List<TechnicianBorrowRequestItemDto> Items { get; set; } = new();
    }

    public class TechnicianBorrowRequestItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Quantity { get; set; }
    }

    public class ApproveTechnicianBorrowRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid RequestId { get; set; }
    }

    public class RejectTechnicianBorrowRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid RequestId { get; set; }
        public string? Reason { get; set; }
    }

    public class UseTechnicianHoldingRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid TechnicianId { get; set; }
        public Guid ProductId { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Assigned;
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    public class ReturnTechnicianHoldingByTypeRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid TechnicianId { get; set; }
        public List<ReturnTechnicianHoldingItem> Items { get; set; } = new();
    }

    public class GetTechnicianHoldingsRequest : IRequest<GetTechnicianHoldingsResponse>
    {
        public Guid? TechnicianId { get; set; }
    }

    public class GetOverdueHoldingsRequest : IRequest<GetTechnicianHoldingsResponse> { }

    public class GetTechnicianHoldingsResponse : BaseResponse
    {
        public List<TechnicianHoldingGroupDto> Technicians { get; set; } = new();

        public GetTechnicianHoldingsResponse(List<TechnicianHoldingGroupDto> technicians)
            : base(true)
        {
            Technicians = technicians;
        }

        public GetTechnicianHoldingsResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class TechnicianHoldingGroupDto
    {
        public Guid TechnicianId { get; set; }
        public string TechnicianName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }
        public int TotalQuantity { get; set; }
        public List<TechnicianHoldingItemDto> Items { get; set; } = new();
    }

    public class TechnicianHoldingItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public TechnicianBorrowType BorrowType { get; set; }
        public string BorrowTypeName { get; set; } = default!;
        public int Quantity { get; set; }
        public DateTime LastBorrowedAt { get; set; }
        public bool IsOverdue { get; set; }
        public string? ReminderMessage { get; set; }
    }

    internal static class TechnicianHoldingRules
    {
        public static Guid? CurrentUserId(IUserSession session)
            => Guid.TryParse(session.UserId, out var userId) ? userId : null;

        public static string BorrowTypeName(TechnicianBorrowType borrowType)
            => borrowType switch
            {
                TechnicianBorrowType.Daily => "Mượn trong ngày",
                TechnicianBorrowType.Assigned or TechnicianBorrowType.Warranty => "Cấp giữ kỹ thuật",
                _ => "Không xác định"
            };

        public static string RequestStatusName(TechnicianBorrowRequestStatus status)
            => status switch
            {
                TechnicianBorrowRequestStatus.Pending => "Chờ thủ kho duyệt",
                TechnicianBorrowRequestStatus.Approved => "Đã duyệt",
                TechnicianBorrowRequestStatus.Rejected => "Đã từ chối",
                TechnicianBorrowRequestStatus.Cancelled => "Đã hủy",
                _ => "Không xác định"
            };

        public static HoldingTransaction NewHoldingTransaction(
            Guid storeId,
            Guid technicianId,
            Guid productId,
            TechnicianBorrowType borrowType,
            HoldingTransactionType type,
            int quantity,
            string? referenceType,
            Guid? referenceId,
            string? note,
            Guid? userId,
            DateTime now)
            => new()
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                TechnicianId = technicianId,
                ProductId = productId,
                BorrowType = borrowType,
                Type = type,
                Quantity = quantity,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Note = note,
                CreatedBy = userId,
                CreatedDate = now
            };
    }

    public class GetSalesByProductReportRequest : IRequest<GetSalesByProductReportResponse>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class GetSalesByProductReportResponse : BaseResponse
    {
        public List<SalesByProductReportRow> Items { get; set; } = new();

        public GetSalesByProductReportResponse(List<SalesByProductReportRow> items)
            : base(true)
        {
            Items = items;
        }

        public GetSalesByProductReportResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class ExportOrderForTechnicianHandler
        : IRequestHandler<ExportOrderForTechnicianRequest, BorrowingMutationResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ExportOrderForTechnicianHandler> _logger;

        public ExportOrderForTechnicianHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            IUnitOfWork uow,
            ILogger<ExportOrderForTechnicianHandler> logger)
        {
            _repo = repo;
            _userSession = userSession;
            _uow = uow;
            _logger = logger;
        }

        public async Task<BorrowingMutationResponse> Handle(
            ExportOrderForTechnicianRequest request,
            CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);
            var isBorrowMoreRequest = false;

            try
            {
                if (request.OrderId == Guid.Empty)
                    return await RollbackAsync("OrderId không hợp lệ.", ct);

                var technicianExports = NormalizeTechnicianExports(request);
                if (technicianExports.Count == 0)
                    return await RollbackAsync("Cần có ít nhất một kỹ thuật và sản phẩm để xuất hàng.", ct);
                if (technicianExports.Any(x => x.TechnicianId == Guid.Empty))
                    return await RollbackAsync("TechnicianId không hợp lệ.", ct);
                if (technicianExports.GroupBy(x => x.TechnicianId).Any(x => x.Count() > 1))
                    return await RollbackAsync("Payload xuất hàng bị trùng kỹ thuật.", ct);
                if (technicianExports.Any(x => x.Items.Count == 0))
                    return await RollbackAsync("Mỗi kỹ thuật cần có ít nhất một sản phẩm để xuất hàng.", ct);
                if (technicianExports
                    .SelectMany(x => x.Items)
                    .Any(x => x.ProductId == Guid.Empty
                        || x.OrderedQuantity < 0
                        || x.BorrowedQuantity < 0
                        || (x.OrderedQuantity == 0 && x.BorrowedQuantity == 0)))
                {
                    return await RollbackAsync("Dữ liệu sản phẩm xuất hàng không hợp lệ.", ct);
                }
                if (technicianExports.Any(x => x.Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1)))
                    return await RollbackAsync("Payload xuất hàng bị trùng sản phẩm trong cùng kỹ thuật.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var order = await _repo.GetOrderWithDetailsAsync(request.OrderId, storeId, ct);
                if (order is null)
                    return await RollbackAsync("Không tìm thấy đơn hàng trong store hiện tại.", ct);
                if (order.Status?.Code is not ("NEW" or "PACKING" or "PACKED"))
                    return await RollbackAsync("Chỉ được xuất hoặc mượn thêm cho kỹ thuật khi đơn hàng chưa hoàn thành hoặc chưa hủy.", ct);

                isBorrowMoreRequest = order.TechnicianExportedAt.HasValue;

                var technicianIds = technicianExports.Select(x => x.TechnicianId).ToList();
                var technicians = await _repo.GetTechniciansByIdsAsync(storeId, technicianIds, ct);
                if (technicians.Count != technicianIds.Count || technicians.Any(x => !x.IsActive))
                    return await RollbackAsync("Có kỹ thuật không tồn tại hoặc đang ngừng hoạt động trong store hiện tại.", ct);

                var details = order.Details.Where(x => !x.IsDeleted).ToList();
                var duplicateProducts = details.GroupBy(x => x.ProductId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (duplicateProducts.Count > 0)
                    return await RollbackAsync("Luồng mượn hàng yêu cầu mỗi sản phẩm chỉ có một dòng trong đơn.", ct);

                var detailByProduct = details.ToDictionary(x => x.ProductId);
                var originalOrderProductIds = details
                    .Where(x => x.Quantity > 0)
                    .Select(x => x.ProductId)
                    .ToHashSet();
                var aggregateByProduct = technicianExports
                    .SelectMany(x => x.Items)
                    .GroupBy(x => x.ProductId)
                    .ToDictionary(
                        x => x.Key,
                        x => new ExportAggregate
                        {
                            ProductId = x.Key,
                            OrderedQuantity = x.Sum(i => i.OrderedQuantity),
                            BorrowedQuantity = x.Sum(i => i.BorrowedQuantity)
                        });

                if (aggregateByProduct.Values.Any(x => x.OrderedQuantity > 0 && !detailByProduct.ContainsKey(x.ProductId)))
                    return await RollbackAsync("Sản phẩm xuất bán phải nằm trong đơn hàng.", ct);

                if (isBorrowMoreRequest)
                {
                    if (aggregateByProduct.Values.Any(x => x.OrderedQuantity > 0))
                    {
                        return await RollbackAsync(
                            "Đơn hàng đã xuất cho kỹ thuật; lần mượn thêm chỉ được nhập SL mượn thêm, không nhập lại SL bán.",
                            ct);
                    }
                }
                else
                {
                    foreach (var detail in detailByProduct.Values.Where(x => x.Quantity > 0))
                    {
                        if (!aggregateByProduct.TryGetValue(detail.ProductId, out var aggregate)
                            || aggregate.OrderedQuantity != detail.Quantity)
                        {
                            return await RollbackAsync(
                                $"Tổng số lượng bán của sản phẩm {detail.ProductName} phải bằng số lượng trong đơn.",
                                ct);
                        }
                    }
                }

                var stocks = await _repo.GetStocksAsync(storeId, aggregateByProduct.Keys, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var aggregate in aggregateByProduct.Values)
                {
                    detailByProduct.TryGetValue(aggregate.ProductId, out var detail);
                    var productCode = detail?.ProductCode ?? aggregate.ProductId.ToString();

                    if (!stockByProduct.TryGetValue(aggregate.ProductId, out var stock))
                        return await RollbackAsync($"Không tìm thấy tồn kho cho sản phẩm {productCode}.", ct);

                    productCode = stock.Product.Code;
                    if (stock.Product.IsSerialTracked)
                        return await RollbackAsync(
                            $"Luồng xuất/mượn cho kỹ thuật chưa hỗ trợ sản phẩm quản lý serial: {stock.Product.Name}.",
                            ct);

                    var totalExportQuantity = aggregate.OrderedQuantity + aggregate.BorrowedQuantity;
                    if (stock.Quantity < totalExportQuantity)
                        return await RollbackAsync(
                            $"Tồn kho thực tế của sản phẩm {stock.Product.Name} không đủ để xuất. Tồn: {stock.Quantity}, cần: {totalExportQuantity}.",
                            ct);

                    var reservedToRelease = Math.Min(stock.Reserved, aggregate.OrderedQuantity);
                    var availableAfterOrdered = (stock.Quantity - aggregate.OrderedQuantity)
                        - (stock.Reserved - reservedToRelease);
                    if (availableAfterOrdered < aggregate.BorrowedQuantity)
                        return await RollbackAsync($"Sản phẩm {productCode} không đủ tồn khả dụng để cho mượn.", ct);

                    if (isBorrowMoreRequest)
                    {
                        var affectedStockRows = await _repo.DecreaseStockAsync(
                            stock.Id,
                            totalExportQuantity,
                            reservedToRelease,
                            userId,
                            now,
                            ct);

                        if (affectedStockRows == 0)
                            return await RollbackAsync($"Tồn kho của sản phẩm {productCode} vừa thay đổi. Vui lòng tải lại đơn hàng rồi mượn thêm lại.", ct);

                        if (detail is null)
                        {
                            detail = new OrderDetail
                            {
                                Id = Guid.NewGuid(),
                                OrderId = order.Id,
                                ProductId = stock.ProductId,
                                ProductName = stock.Product.Name,
                                ProductCode = stock.Product.Code,
                                Unit = stock.Product.Unit,
                                Quantity = 0,
                                BorrowedQuantity = aggregate.BorrowedQuantity,
                                UsedBorrowedQuantity = 0,
                                UnitPrice = stock.Product.SalePrice.GetValueOrDefault(),
                                Total = 0,
                                CreatedBy = userId,
                                CreatedDate = now,
                                RowVersion = Guid.NewGuid()
                            };

                            _repo.AddOrderDetail(detail);
                            detailByProduct[aggregate.ProductId] = detail;
                        }
                        else
                        {
                            var affectedDetailRows = await _repo.IncreaseOrderDetailBorrowedQuantityAsync(
                                detail.Id,
                                aggregate.BorrowedQuantity,
                                userId,
                                now,
                                ct);

                            if (affectedDetailRows == 0)
                                return await RollbackAsync($"Dòng sản phẩm {productCode} trong đơn vừa thay đổi. Vui lòng tải lại đơn hàng rồi mượn thêm lại.", ct);
                        }

                        continue;
                    }

                    stock.Quantity -= totalExportQuantity;
                    stock.Reserved -= reservedToRelease;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;
                    stock.RowVersion = Guid.NewGuid();

                    if (detail is null)
                    {
                        detail = new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            ProductId = stock.ProductId,
                            ProductName = stock.Product.Name,
                            ProductCode = stock.Product.Code,
                            Unit = stock.Product.Unit,
                            Quantity = 0,
                            BorrowedQuantity = aggregate.BorrowedQuantity,
                            UsedBorrowedQuantity = 0,
                            UnitPrice = stock.Product.SalePrice.GetValueOrDefault(),
                            Total = 0,
                            CreatedBy = userId,
                            CreatedDate = now,
                            RowVersion = Guid.NewGuid()
                        };

                        order.Details.Add(detail);
                        detailByProduct[aggregate.ProductId] = detail;
                    }
                    else
                    {
                        detail.BorrowedQuantity = isBorrowMoreRequest
                            ? detail.BorrowedQuantity + aggregate.BorrowedQuantity
                            : aggregate.BorrowedQuantity;
                        detail.UpdatedBy = userId;
                        detail.UpdatedDate = now;
                        detail.RowVersion = Guid.NewGuid();
                    }
                }

                foreach (var technicianExport in technicianExports)
                {
                    foreach (var item in technicianExport.Items.Where(x => x.BorrowedQuantity > 0))
                    {
                        var holding = await _repo.GetHoldingAsync(storeId, technicianExport.TechnicianId, item.ProductId, ct);
                        if (holding is null)
                        {
                            holding = new BizMate.Domain.Entities.TechnicianHolding
                            {
                                Id = Guid.NewGuid(),
                                StoreId = storeId,
                                TechnicianId = technicianExport.TechnicianId,
                                ProductId = item.ProductId,
                                Quantity = item.BorrowedQuantity,
                                LastBorrowedAt = now,
                                CreatedBy = userId,
                                CreatedDate = now,
                                RowVersion = Guid.NewGuid()
                            };
                            _repo.AddHolding(holding);
                        }
                        else
                        {
                            if (isBorrowMoreRequest)
                            {
                                var affectedHoldingRows = await _repo.IncreaseHoldingQuantityAsync(
                                    holding.Id,
                                    item.BorrowedQuantity,
                                    userId,
                                    now,
                                    ct);

                                if (affectedHoldingRows == 0)
                                    return await RollbackAsync("Hàng kỹ thuật đang giữ vừa thay đổi. Vui lòng tải lại đơn hàng rồi mượn thêm lại.", ct);
                            }
                            else
                            {
                                holding.Quantity += item.BorrowedQuantity;
                                holding.LastBorrowedAt = now;
                                holding.UpdatedBy = userId;
                                holding.UpdatedDate = now;
                                holding.RowVersion = Guid.NewGuid();
                            }
                        }

                        _repo.AddTransaction(NewTransaction(
                            storeId,
                            technicianExport.TechnicianId,
                            item.ProductId,
                            HoldingTransactionType.Borrow,
                            item.BorrowedQuantity,
                            "order",
                            order.Id,
                            originalOrderProductIds.Contains(item.ProductId)
                                ? "Borrowed with order export"
                                : "Borrowed extra product with order export",
                            userId,
                            now));
                    }
                }

                if (!isBorrowMoreRequest)
                {
                    AddMissingOrderTechnicians(order, technicianIds, now);
                    order.TechnicianId ??= technicianIds.First();
                    order.TechnicianExportedAt = now;
                    order.UpdatedBy = userId;
                    order.UpdatedDate = now;
                    order.RowVersion = Guid.NewGuid();
                    order.RecalculateTotal();
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
                return new BorrowingMutationResponse(
                    true,
                    isBorrowMoreRequest
                        ? "Ghi nhận mượn thêm sản phẩm thành công."
                        : "Xuất hàng và ghi nhận hàng mượn thành công.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _uow.RollbackAsync(ct);
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict while exporting order {OrderId} for technician. Entries: {@Entries}",
                    request.OrderId,
                    DescribeConcurrencyEntries(ex));
                return new BorrowingMutationResponse(
                    false,
                    isBorrowMoreRequest
                        ? "Dữ liệu đơn hàng hoặc tồn kho vừa thay đổi. Vui lòng tải lại đơn hàng rồi mượn thêm lại."
                        : "Dữ liệu đơn hàng hoặc tồn kho vừa thay đổi. Vui lòng tải lại đơn hàng rồi xuất lại.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to export order {OrderId} for technician.", request.OrderId);
                return new BorrowingMutationResponse(false, "Không thể xuất hàng cho kỹ thuật. Vui lòng kiểm tra tồn kho và thử lại.");
            }
        }

        private static List<NormalizedTechnicianExport> NormalizeTechnicianExports(
            ExportOrderForTechnicianRequest request)
        {
            if (request.TechnicianExports.Count > 0)
            {
                return request.TechnicianExports
                    .Select(x => new NormalizedTechnicianExport
                    {
                        TechnicianId = x.TechnicianId,
                        Items = x.Items.ToList()
                    })
                    .ToList();
            }

            if (request.Items.Count == 0)
                return new List<NormalizedTechnicianExport>();

            return new List<NormalizedTechnicianExport>
            {
                new()
                {
                    TechnicianId = request.TechnicianId ?? Guid.Empty,
                    Items = request.Items.ToList()
                }
            };
        }

        private static void AddMissingOrderTechnicians(
            BizMate.Domain.Entities.Order order,
            IEnumerable<Guid> technicianIds,
            DateTime now)
        {
            var existingIds = order.OrderTechnicians
                .Select(x => x.TechnicianId)
                .ToHashSet();

            foreach (var technicianId in technicianIds.Where(x => !existingIds.Contains(x)))
            {
                order.OrderTechnicians.Add(new OrderTechnician
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    TechnicianId = technicianId,
                    AssignedAt = now,
                    CreatedDate = now
                });
            }
        }

        private sealed class NormalizedTechnicianExport
        {
            public Guid TechnicianId { get; set; }
            public List<ExportOrderForTechnicianItem> Items { get; set; } = new();
        }

        private sealed class ExportAggregate
        {
            public Guid ProductId { get; set; }
            public int OrderedQuantity { get; set; }
            public int BorrowedQuantity { get; set; }
        }

        private static List<object> DescribeConcurrencyEntries(DbUpdateConcurrencyException exception)
            => exception.Entries
                .Select(entry => new
                {
                    Entity = entry.Metadata.ClrType.Name,
                    State = entry.State.ToString(),
                    Id = entry.Properties.FirstOrDefault(x => x.Metadata.Name == nameof(BaseCoreEntity.Id))?.CurrentValue,
                    RowVersion = entry.Properties.FirstOrDefault(x => x.Metadata.Name == nameof(BaseCoreEntity.RowVersion))?.CurrentValue
                })
                .Cast<object>()
                .ToList();

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await _uow.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }

        private static Guid? CurrentUserId(IUserSession userSession)
            => Guid.TryParse(userSession.UserId, out var userId) ? userId : null;

        private static HoldingTransaction NewTransaction(
            Guid storeId,
            Guid technicianId,
            Guid productId,
            HoldingTransactionType type,
            int quantity,
            string? referenceType,
            Guid? referenceId,
            string? note,
            Guid? userId,
            DateTime now)
            => new()
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                TechnicianId = technicianId,
                ProductId = productId,
                Type = type,
                Quantity = quantity,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Note = note,
                CreatedBy = userId,
                CreatedDate = now
            };
    }

    public class CreateTechnicianBorrowRequestHandler(
        ITechnicianHoldingRepository repo,
        IProductRepository productRepository,
        ICodeGeneratorService codeGeneratorService,
        IUserSession userSession,
        IUnitOfWork unitOfWork,
        ILogger<CreateTechnicianBorrowRequestHandler> logger)
        : IRequestHandler<CreateTechnicianBorrowRequest, BorrowingMutationResponse>
    {
        public async Task<BorrowingMutationResponse> Handle(CreateTechnicianBorrowRequest request, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                if (request.BorrowType is not (
                    TechnicianBorrowType.Daily or TechnicianBorrowType.Assigned))
                    return await RollbackAsync("Loại mượn không hợp lệ.", ct);
                if (request.TechnicianId == Guid.Empty && CurrentUserId(userSession) is null)
                    return await RollbackAsync("Vui lòng chọn kỹ thuật viên.", ct);
                if (request.Items.Count == 0 || request.Items.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0))
                    return await RollbackAsync("Danh sách sản phẩm mượn không hợp lệ.", ct);
                if (request.Items.GroupBy(x => x.ProductId).Any(x => x.Count() > 1))
                    return await RollbackAsync("Danh sách sản phẩm mượn bị trùng sản phẩm.", ct);

                var storeId = userSession.StoreId;
                var userId = CurrentUserId(userSession);
                var currentTechnician = userId.HasValue
                    ? await repo.GetTechnicianByUserIdAsync(userId.Value, storeId, ct)
                    : null;
                var technicianId = currentTechnician?.Id ?? request.TechnicianId;
                if (technicianId == Guid.Empty)
                    return await RollbackAsync("Tài khoản chưa được liên kết với hồ sơ kỹ thuật viên.", ct);

                var technician = currentTechnician
                    ?? await repo.GetTechnicianAsync(technicianId, storeId, ct);
                if (technician is null || !technician.IsActive)
                    return await RollbackAsync("Kỹ thuật viên không tồn tại hoặc đang ngừng hoạt động.", ct);

                var productIds = request.Items.Select(x => x.ProductId).ToList();
                var products = await productRepository.GetByIdsAsync(productIds, ct);
                var productById = products.ToDictionary(x => x.Id);
                if (productById.Count != productIds.Count)
                    return await RollbackAsync("Có sản phẩm mượn không tồn tại.", ct);
                if (products.Any(x => x.IsSerialTracked))
                    return await RollbackAsync("Mượn hàng kỹ thuật hiện chưa hỗ trợ sản phẩm quản lý serial.", ct);

                var requestId = Guid.NewGuid();
                var now = DateTime.UtcNow;
                var borrowRequest = new TechnicianBorrowRequest
                {
                    Id = requestId,
                    StoreId = storeId,
                    Code = await codeGeneratorService.GenerateCodeAsync("#MH", 5),
                    TechnicianId = technicianId,
                    BorrowType = request.BorrowType,
                    RequestStatus = TechnicianBorrowRequestStatus.Pending,
                    NeededDate = request.NeededDate,
                    Description = request.Description?.Trim(),
                    CreatedBy = userId,
                    CreatedDate = now,
                    UpdatedBy = userId,
                    UpdatedDate = now,
                    IsActive = true,
                    Items = request.Items.Select(item =>
                    {
                        var product = productById[item.ProductId];
                        return new TechnicianBorrowRequestItem
                        {
                            Id = Guid.NewGuid(),
                            TechnicianBorrowRequestId = requestId,
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            Quantity = item.Quantity,
                            CreatedBy = userId,
                            CreatedDate = now,
                            UpdatedBy = userId,
                            UpdatedDate = now
                        };
                    }).ToList()
                };

                repo.AddBorrowRequest(borrowRequest);
                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Đã gửi đề xuất mượn hàng cho thủ kho duyệt.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(ex, "Lỗi khi tạo đề xuất mượn hàng kỹ thuật.");
                return new BorrowingMutationResponse(false, "Không thể tạo đề xuất mượn hàng. Vui lòng thử lại.");
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await unitOfWork.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }

        private static Guid? CurrentUserId(IUserSession session)
            => Guid.TryParse(session.UserId, out var userId) ? userId : null;
    }

    public class GetTechnicianBorrowRequestsHandler(ITechnicianHoldingRepository repo, IUserSession userSession)
        : IRequestHandler<GetTechnicianBorrowRequestsRequest, GetTechnicianBorrowRequestsResponse>
    {
        public async Task<GetTechnicianBorrowRequestsResponse> Handle(GetTechnicianBorrowRequestsRequest request, CancellationToken ct)
        {
            var userId = TechnicianHoldingRules.CurrentUserId(userSession);
            var currentTechnician = userId.HasValue
                ? await repo.GetTechnicianByUserIdAsync(
                    userId.Value,
                    userSession.StoreId,
                    ct)
                : null;
            var technicianId = currentTechnician?.Id ?? request.TechnicianId;
            var requests = await repo.GetBorrowRequestsAsync(
                userSession.StoreId,
                request.Status,
                technicianId,
                ct);

            return new GetTechnicianBorrowRequestsResponse(requests.Select(ToDto).ToList());
        }

        private static TechnicianBorrowRequestDto ToDto(TechnicianBorrowRequest request)
            => new()
            {
                Id = request.Id,
                Code = request.Code,
                TechnicianId = request.TechnicianId,
                TechnicianName = request.Technician.Name,
                Phone = request.Technician.Phone,
                BorrowType = request.BorrowType,
                BorrowTypeName = TechnicianHoldingRules.BorrowTypeName(request.BorrowType),
                RequestStatus = request.RequestStatus,
                RequestStatusName = TechnicianHoldingRules.RequestStatusName(request.RequestStatus),
                NeededDate = request.NeededDate,
                CreatedDate = request.CreatedDate,
                ApprovedAt = request.ApprovedAt,
                Description = request.Description,
                RejectionReason = request.RejectionReason,
                TotalQuantity = request.Items.Sum(x => x.Quantity),
                Items = request.Items.Select(x => new TechnicianBorrowRequestItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    Quantity = x.Quantity
                }).ToList()
            };
    }

    public class ApproveTechnicianBorrowRequestHandler(
        ITechnicianHoldingRepository repo,
        IUserSession userSession,
        IUnitOfWork unitOfWork,
        ILogger<ApproveTechnicianBorrowRequestHandler> logger)
        : IRequestHandler<ApproveTechnicianBorrowRequest, BorrowingMutationResponse>
    {
        public async Task<BorrowingMutationResponse> Handle(ApproveTechnicianBorrowRequest request, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                if (request.RequestId == Guid.Empty)
                    return await RollbackAsync("Đề xuất mượn hàng không hợp lệ.", ct);

                var storeId = userSession.StoreId;
                var userId = TechnicianHoldingRules.CurrentUserId(userSession);
                var now = DateTime.UtcNow;
                var borrowRequest = await repo.GetBorrowRequestAsync(storeId, request.RequestId, ct);
                if (borrowRequest is null)
                    return await RollbackAsync("Không tìm thấy đề xuất mượn hàng.", ct);
                if (borrowRequest.RequestStatus != TechnicianBorrowRequestStatus.Pending)
                    return await RollbackAsync("Chỉ được duyệt đề xuất đang chờ duyệt.", ct);
                if (!borrowRequest.Technician.IsActive)
                    return await RollbackAsync("Kỹ thuật viên đang ngừng hoạt động.", ct);
                if (borrowRequest.Items.Count == 0)
                    return await RollbackAsync("Đề xuất chưa có sản phẩm.", ct);

                var productIds = borrowRequest.Items.Select(x => x.ProductId).Distinct().ToList();
                var stocks = await repo.GetStocksAsync(storeId, productIds, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var item in borrowRequest.Items)
                {
                    if (!stockByProduct.TryGetValue(item.ProductId, out var stock))
                        return await RollbackAsync($"Không tìm thấy tồn kho cho sản phẩm {item.ProductCode}.", ct);
                    if (stock.Product.IsSerialTracked)
                        return await RollbackAsync($"Mượn hàng kỹ thuật hiện chưa hỗ trợ sản phẩm quản lý serial: {stock.Product.Name}.", ct);

                    if (borrowRequest.BorrowType == TechnicianBorrowType.Daily)
                    {
                        if (stock.Available < item.Quantity)
                            return await RollbackAsync($"Sản phẩm {stock.Product.Name} không đủ tồn khả dụng. Khả dụng {stock.Available}, cần {item.Quantity}.", ct);

                        stock.Reserved += item.Quantity;
                    }
                    else
                    {
                        if (stock.Available < item.Quantity)
                            return await RollbackAsync($"Sản phẩm {stock.Product.Name} không đủ tồn khả dụng để cấp cho kỹ thuật. Khả dụng {stock.Available}, cần {item.Quantity}.", ct);

                        stock.Quantity -= item.Quantity;
                    }

                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;
                    stock.RowVersion = Guid.NewGuid();

                    var holding = await repo.GetHoldingByTypeAsync(
                        storeId,
                        borrowRequest.TechnicianId,
                        item.ProductId,
                        borrowRequest.BorrowType,
                        ct);

                    if (holding is null)
                    {
                        repo.AddHolding(new BizMate.Domain.Entities.TechnicianHolding
                        {
                            Id = Guid.NewGuid(),
                            StoreId = storeId,
                            TechnicianId = borrowRequest.TechnicianId,
                            ProductId = item.ProductId,
                            BorrowType = borrowRequest.BorrowType,
                            Quantity = item.Quantity,
                            LastBorrowedAt = now,
                            CreatedBy = userId,
                            CreatedDate = now,
                            UpdatedBy = userId,
                            UpdatedDate = now
                        });
                    }
                    else
                    {
                        holding.Quantity += item.Quantity;
                        holding.LastBorrowedAt = now;
                        holding.UpdatedBy = userId;
                        holding.UpdatedDate = now;
                        holding.RowVersion = Guid.NewGuid();
                    }

                    repo.AddTransaction(TechnicianHoldingRules.NewHoldingTransaction(
                        storeId,
                        borrowRequest.TechnicianId,
                        item.ProductId,
                        borrowRequest.BorrowType,
                        HoldingTransactionType.Borrow,
                        item.Quantity,
                        "technician-borrow-request",
                        borrowRequest.Id,
                        $"Approved borrow request {borrowRequest.Code}",
                        userId,
                        now));
                }

                borrowRequest.RequestStatus = TechnicianBorrowRequestStatus.Approved;
                borrowRequest.ApprovedAt = now;
                borrowRequest.ApprovedBy = userId;
                borrowRequest.UpdatedBy = userId;
                borrowRequest.UpdatedDate = now;
                borrowRequest.RowVersion = Guid.NewGuid();

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Đã duyệt và xuất hàng cho kỹ thuật.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(ex, "Lỗi khi duyệt đề xuất mượn hàng kỹ thuật.");
                return new BorrowingMutationResponse(false, "Không thể duyệt đề xuất mượn hàng. Vui lòng thử lại.");
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await unitOfWork.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }
    }

    public class RejectTechnicianBorrowRequestHandler(
        ITechnicianHoldingRepository repo,
        IUserSession userSession,
        IUnitOfWork unitOfWork,
        ILogger<RejectTechnicianBorrowRequestHandler> logger)
        : IRequestHandler<RejectTechnicianBorrowRequest, BorrowingMutationResponse>
    {
        public async Task<BorrowingMutationResponse> Handle(RejectTechnicianBorrowRequest request, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var borrowRequest = await repo.GetBorrowRequestAsync(userSession.StoreId, request.RequestId, ct);
                if (borrowRequest is null)
                    return await RollbackAsync("Không tìm thấy đề xuất mượn hàng.", ct);
                if (borrowRequest.RequestStatus != TechnicianBorrowRequestStatus.Pending)
                    return await RollbackAsync("Chỉ được từ chối đề xuất đang chờ duyệt.", ct);

                var userId = TechnicianHoldingRules.CurrentUserId(userSession);
                borrowRequest.RequestStatus = TechnicianBorrowRequestStatus.Rejected;
                borrowRequest.RejectionReason = request.Reason?.Trim();
                borrowRequest.UpdatedBy = userId;
                borrowRequest.UpdatedDate = DateTime.UtcNow;
                borrowRequest.RowVersion = Guid.NewGuid();

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Đã từ chối đề xuất mượn hàng.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(ex, "Lỗi khi từ chối đề xuất mượn hàng kỹ thuật.");
                return new BorrowingMutationResponse(false, "Không thể từ chối đề xuất mượn hàng. Vui lòng thử lại.");
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await unitOfWork.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }
    }

    public class UseTechnicianHoldingHandler(
        ITechnicianHoldingRepository repo,
        IUserSession userSession,
        IUnitOfWork unitOfWork,
        ILogger<UseTechnicianHoldingHandler> logger)
        : IRequestHandler<UseTechnicianHoldingRequest, BorrowingMutationResponse>
    {
        public async Task<BorrowingMutationResponse> Handle(UseTechnicianHoldingRequest request, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                if (request.TechnicianId == Guid.Empty || request.ProductId == Guid.Empty || request.Quantity <= 0)
                    return await RollbackAsync("Dữ liệu ghi nhận sử dụng không hợp lệ.", ct);
                if (!Enum.IsDefined(request.BorrowType))
                    return await RollbackAsync("Loại mượn không hợp lệ.", ct);

                var storeId = userSession.StoreId;
                var userId = TechnicianHoldingRules.CurrentUserId(userSession);
                var now = DateTime.UtcNow;
                var holding = await repo.GetHoldingByTypeAsync(
                    storeId,
                    request.TechnicianId,
                    request.ProductId,
                    request.BorrowType,
                    ct);

                if (holding is null || holding.Quantity < request.Quantity)
                    return await RollbackAsync("Số lượng sử dụng vượt quá số lượng kỹ thuật đang giữ.", ct);

                var stocks = await repo.GetStocksAsync(storeId, new[] { request.ProductId }, ct);
                var stock = stocks.FirstOrDefault();
                if (stock is null)
                    return await RollbackAsync("Không tìm thấy tồn kho sản phẩm.", ct);

                if (request.BorrowType == TechnicianBorrowType.Daily)
                {
                    if (stock.Quantity < request.Quantity || stock.Reserved < request.Quantity)
                        return await RollbackAsync("Tồn kho đã thay đổi, vui lòng tải lại dữ liệu.", ct);

                    stock.Quantity -= request.Quantity;
                    stock.Reserved -= request.Quantity;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;
                    stock.RowVersion = Guid.NewGuid();
                }

                holding.Quantity -= request.Quantity;
                holding.UpdatedBy = userId;
                holding.UpdatedDate = now;
                holding.RowVersion = Guid.NewGuid();

                repo.AddTransaction(TechnicianHoldingRules.NewHoldingTransaction(
                    storeId,
                    request.TechnicianId,
                    request.ProductId,
                    request.BorrowType,
                    HoldingTransactionType.ConvertToSale,
                    request.Quantity,
                    "technician-holding",
                    null,
                    string.IsNullOrWhiteSpace(request.Note) ? "Technician used borrowed item" : request.Note.Trim(),
                    userId,
                    now));

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Đã ghi nhận kỹ thuật sử dụng hàng mượn.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(ex, "Lỗi khi ghi nhận sử dụng hàng kỹ thuật.");
                return new BorrowingMutationResponse(false, "Không thể ghi nhận sử dụng hàng mượn. Vui lòng thử lại.");
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await unitOfWork.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }
    }

    public class ReturnTechnicianHoldingByTypeHandler(
        ITechnicianHoldingRepository repo,
        IUserSession userSession,
        IUnitOfWork unitOfWork,
        ILogger<ReturnTechnicianHoldingByTypeHandler> logger)
        : IRequestHandler<ReturnTechnicianHoldingByTypeRequest, BorrowingMutationResponse>
    {
        public async Task<BorrowingMutationResponse> Handle(ReturnTechnicianHoldingByTypeRequest request, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                if (request.TechnicianId == Guid.Empty)
                    return await RollbackAsync("Kỹ thuật viên không hợp lệ.", ct);
                if (request.Items.Count == 0
                    || request.Items.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0 || !Enum.IsDefined(x.BorrowType)))
                    return await RollbackAsync("Dữ liệu trả hàng không hợp lệ.", ct);
                if (request.Items.GroupBy(x => new { x.ProductId, x.BorrowType }).Any(x => x.Count() > 1))
                    return await RollbackAsync("Payload trả hàng bị trùng sản phẩm và loại mượn.", ct);

                var storeId = userSession.StoreId;
                var userId = TechnicianHoldingRules.CurrentUserId(userSession);
                var now = DateTime.UtcNow;

                var technician = await repo.GetTechnicianAsync(request.TechnicianId, storeId, ct);
                if (technician is null)
                    return await RollbackAsync("Không tìm thấy kỹ thuật viên trong store hiện tại.", ct);

                var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
                var stocks = await repo.GetStocksAsync(storeId, productIds, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var item in request.Items)
                {
                    var holding = await repo.GetHoldingByTypeAsync(storeId, technician.Id, item.ProductId, item.BorrowType, ct);
                    if (holding is null || holding.Quantity < item.Quantity)
                        return await RollbackAsync("Số lượng trả vượt quá số lượng kỹ thuật đang giữ.", ct);
                    if (!stockByProduct.TryGetValue(item.ProductId, out var stock))
                        return await RollbackAsync("Không tìm thấy tồn kho để nhập lại hàng trả.", ct);

                    holding.Quantity -= item.Quantity;
                    holding.UpdatedBy = userId;
                    holding.UpdatedDate = now;
                    holding.RowVersion = Guid.NewGuid();

                    if (item.BorrowType == TechnicianBorrowType.Daily)
                    {
                        stock.Reserved -= item.Quantity;
                        if (stock.Reserved < 0)
                            return await RollbackAsync("Tồn kho giữ chỗ đã thay đổi, vui lòng tải lại dữ liệu.", ct);
                    }
                    else
                    {
                        stock.Quantity += item.Quantity;
                    }

                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;
                    stock.RowVersion = Guid.NewGuid();

                    repo.AddTransaction(TechnicianHoldingRules.NewHoldingTransaction(
                        storeId,
                        technician.Id,
                        item.ProductId,
                        item.BorrowType,
                        HoldingTransactionType.Return,
                        item.Quantity,
                        "return",
                        null,
                        "Returned by technician",
                        userId,
                        now));
                }

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Trả hàng thành công.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                logger.LogError(ex, "Lỗi khi trả hàng kỹ thuật.");
                return new BorrowingMutationResponse(false, "Không thể trả hàng. Vui lòng thử lại.");
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await unitOfWork.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }
    }

    public class ReturnTechnicianHoldingHandler
        : IRequestHandler<ReturnTechnicianHoldingRequest, BorrowingMutationResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;

        public ReturnTechnicianHoldingHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _repo = repo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<BorrowingMutationResponse> Handle(ReturnTechnicianHoldingRequest request, CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);

            try
            {
                if (request.TechnicianId == Guid.Empty)
                    return await RollbackAsync("TechnicianId không hợp lệ.", ct);
                if (request.Items.Count == 0 || request.Items.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0))
                    return await RollbackAsync("Dữ liệu trả hàng không hợp lệ.", ct);
                if (request.Items.GroupBy(x => x.ProductId).Any(x => x.Count() > 1))
                    return await RollbackAsync("Payload trả hàng bị trùng sản phẩm.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var technician = await _repo.GetTechnicianAsync(request.TechnicianId, storeId, ct);
                if (technician is null)
                    return await RollbackAsync("Không tìm thấy kỹ thuật trong store hiện tại.", ct);

                var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
                var stocks = await _repo.GetStocksAsync(storeId, productIds, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var item in request.Items)
                {
                    var holding = await _repo.GetHoldingAsync(storeId, technician.Id, item.ProductId, ct);
                    if (holding is null || holding.Quantity < item.Quantity)
                        return await RollbackAsync("Số lượng trả vượt quá số lượng kỹ thuật đang giữ.", ct);
                    if (!stockByProduct.TryGetValue(item.ProductId, out var stock))
                        return await RollbackAsync("Không tìm thấy tồn kho để nhập lại hàng trả.", ct);

                    holding.Quantity -= item.Quantity;
                    holding.UpdatedBy = userId;
                    holding.UpdatedDate = now;

                    stock.Quantity += item.Quantity;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;

                    _repo.AddTransaction(NewTransaction(
                        storeId,
                        technician.Id,
                        item.ProductId,
                        HoldingTransactionType.Return,
                        item.Quantity,
                        "return",
                        null,
                        "Returned by technician",
                        userId,
                        now));
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Trả hàng thành công.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync(ct);
                return new BorrowingMutationResponse(false, ex.Message);
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await _uow.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }

        private static Guid? CurrentUserId(IUserSession userSession)
            => Guid.TryParse(userSession.UserId, out var userId) ? userId : null;

        private static HoldingTransaction NewTransaction(
            Guid storeId,
            Guid technicianId,
            Guid productId,
            HoldingTransactionType type,
            int quantity,
            string? referenceType,
            Guid? referenceId,
            string? note,
            Guid? userId,
            DateTime now)
            => new()
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                TechnicianId = technicianId,
                ProductId = productId,
                Type = type,
                Quantity = quantity,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Note = note,
                CreatedBy = userId,
                CreatedDate = now
            };
    }

    public class UseBorrowedItemHandler
        : IRequestHandler<UseBorrowedItemRequest, BorrowingMutationResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;

        public UseBorrowedItemHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _repo = repo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<BorrowingMutationResponse> Handle(UseBorrowedItemRequest request, CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);

            try
            {
                if (request.OrderId == Guid.Empty || request.ProductId == Guid.Empty || request.Quantity <= 0)
                    return await RollbackAsync("Dữ liệu dùng hàng mượn không hợp lệ.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var order = await _repo.GetOrderWithDetailsAsync(request.OrderId, storeId, ct);
                if (order is null)
                    return await RollbackAsync("Không tìm thấy đơn hàng trong store hiện tại.", ct);

                if (order.Status?.Code is "COMPLETED" or "CANCELLED")
                    return await RollbackAsync("\u0110\u01a1n h\u00e0ng \u0111\u00e3 ho\u00e0n th\u00e0nh ho\u1eb7c \u0111\u00e3 h\u1ee7y, kh\u00f4ng th\u1ec3 ghi nh\u1eadn d\u00f9ng h\u00e0ng m\u01b0\u1ee3n.", ct);

                var productHoldings = await _repo.GetHoldingsByProductAsync(storeId, request.ProductId, ct);
                var technicianId = ResolveTechnicianId(order, request.TechnicianId, productHoldings);
                if (!technicianId.HasValue)
                    return await RollbackAsync("Cần chọn kỹ thuật dùng hàng mượn cho đơn có nhiều kỹ thuật.", ct);

                var detail = order.Details.FirstOrDefault(x => x.ProductId == request.ProductId && !x.IsDeleted);
                if (detail is null)
                    return await RollbackAsync("Sản phẩm không thuộc đơn hàng.", ct);

                var remainingBorrowed = detail.BorrowedQuantity - detail.UsedBorrowedQuantity;
                if (remainingBorrowed < request.Quantity)
                    return await RollbackAsync("Số lượng dùng vượt quá số lượng đã mượn còn lại của đơn.", ct);

                var holding = await _repo.GetHoldingAsync(storeId, technicianId.Value, request.ProductId, ct);
                if (holding is null || holding.Quantity < request.Quantity)
                    return await RollbackAsync("Kỹ thuật viên không còn đủ hàng mượn để sử dụng.", ct);

                detail.UsedBorrowedQuantity += request.Quantity;
                detail.UpdatedBy = userId;
                detail.UpdatedDate = now;

                holding.Quantity -= request.Quantity;
                holding.UpdatedBy = userId;
                holding.UpdatedDate = now;

                _repo.AddTransaction(new HoldingTransaction
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    TechnicianId = technicianId.Value,
                    ProductId = request.ProductId,
                    Type = HoldingTransactionType.ConvertToSale,
                    Quantity = request.Quantity,
                    ReferenceType = "order",
                    ReferenceId = order.Id,
                    Note = "Borrowed item converted to sale",
                    CreatedBy = userId,
                    CreatedDate = now
                });

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Đã chuyển hàng mượn thành hàng bán.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync(ct);
                return new BorrowingMutationResponse(false, ex.Message);
            }
        }

        private async Task<BorrowingMutationResponse> RollbackAsync(string message, CancellationToken ct)
        {
            await _uow.RollbackAsync(ct);
            return new BorrowingMutationResponse(false, message);
        }

        private static Guid? CurrentUserId(IUserSession userSession)
            => Guid.TryParse(userSession.UserId, out var userId) ? userId : null;

        private static Guid? ResolveTechnicianId(
            BizMate.Domain.Entities.Order order,
            Guid? requestedTechnicianId,
            IReadOnlyCollection<BizMate.Domain.Entities.TechnicianHolding> productHoldings)
        {
            var assignedIds = order.OrderTechnicians
                .Select(x => x.TechnicianId)
                .Distinct()
                .ToList();

            var holdingTechnicianIds = productHoldings
                .Select(x => x.TechnicianId)
                .Distinct()
                .ToList();

            if (requestedTechnicianId.HasValue && requestedTechnicianId.Value != Guid.Empty)
            {
                if (assignedIds.Count > 0 && !assignedIds.Contains(requestedTechnicianId.Value))
                    return null;

                return requestedTechnicianId.Value;
            }

            if (holdingTechnicianIds.Count > 1 || assignedIds.Count > 1)
                return null;

            if (holdingTechnicianIds.Count == 1)
                return holdingTechnicianIds[0];

            if (assignedIds.Count == 1)
                return assignedIds[0];

            return order.TechnicianId;
        }
    }

    public class GetTechnicianHoldingsHandler
        : IRequestHandler<GetTechnicianHoldingsRequest, GetTechnicianHoldingsResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;

        public GetTechnicianHoldingsHandler(ITechnicianHoldingRepository repo, IUserSession userSession)
        {
            _repo = repo;
            _userSession = userSession;
        }

        public async Task<GetTechnicianHoldingsResponse> Handle(GetTechnicianHoldingsRequest request, CancellationToken ct)
        {
            var userId = TechnicianHoldingRules.CurrentUserId(_userSession);
            var currentTechnician = userId.HasValue
                ? await _repo.GetTechnicianByUserIdAsync(
                    userId.Value,
                    _userSession.StoreId,
                    ct)
                : null;
            var technicianId = currentTechnician?.Id ?? request.TechnicianId;
            var holdings = await _repo.GetHoldingsAsync(_userSession.StoreId, technicianId, ct);
            return new GetTechnicianHoldingsResponse(ToGroups(holdings, DateTime.UtcNow.AddHours(-24)));
        }

        internal static List<TechnicianHoldingGroupDto> ToGroups(
            IEnumerable<BizMate.Domain.Entities.TechnicianHolding> holdings,
            DateTime overdueBefore)
        {
            return holdings
                .GroupBy(x => x.TechnicianId)
                .Select(g =>
                {
                    var first = g.First();
                    var items = g.Select(x =>
                    {
                        var isOverdue = x.BorrowType == TechnicianBorrowType.Daily
                            && x.Quantity > 0
                            && x.LastBorrowedAt <= overdueBefore;
                        return new TechnicianHoldingItemDto
                        {
                            ProductId = x.ProductId,
                            ProductName = x.Product.Name,
                            ProductCode = x.Product.Code,
                            BorrowType = x.BorrowType,
                            BorrowTypeName = TechnicianHoldingRules.BorrowTypeName(x.BorrowType),
                            Quantity = x.Quantity,
                            LastBorrowedAt = x.LastBorrowedAt,
                            IsOverdue = isOverdue,
                            ReminderMessage = isOverdue
                                ? $"Kỹ thuật {x.Technician.Name} còn giữ {x.Quantity} {x.Product.Name}, vui lòng trả hàng cuối ngày."
                                : null
                        };
                    }).ToList();

                    return new TechnicianHoldingGroupDto
                    {
                        TechnicianId = first.TechnicianId,
                        TechnicianName = first.Technician.Name,
                        Phone = first.Technician.Phone,
                        ZaloPhone = first.Technician.ZaloPhone,
                        TotalQuantity = items.Sum(x => x.Quantity),
                        Items = items
                    };
                })
                .OrderBy(x => x.TechnicianName)
                .ToList();
        }
    }

    public class GetOverdueHoldingsHandler
        : IRequestHandler<GetOverdueHoldingsRequest, GetTechnicianHoldingsResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;

        public GetOverdueHoldingsHandler(ITechnicianHoldingRepository repo, IUserSession userSession)
        {
            _repo = repo;
            _userSession = userSession;
        }

        public async Task<GetTechnicianHoldingsResponse> Handle(GetOverdueHoldingsRequest request, CancellationToken ct)
        {
            var overdueBefore = DateTime.UtcNow.AddHours(-24);
            var holdings = await _repo.GetOverdueHoldingsAsync(_userSession.StoreId, overdueBefore, ct);
            return new GetTechnicianHoldingsResponse(
                GetTechnicianHoldingsHandler.ToGroups(holdings, overdueBefore));
        }
    }

    public class GetSalesByProductReportHandler
        : IRequestHandler<GetSalesByProductReportRequest, GetSalesByProductReportResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;

        public GetSalesByProductReportHandler(ITechnicianHoldingRepository repo, IUserSession userSession)
        {
            _repo = repo;
            _userSession = userSession;
        }

        public async Task<GetSalesByProductReportResponse> Handle(GetSalesByProductReportRequest request, CancellationToken ct)
        {
            var rows = await _repo.GetSalesByProductAsync(
                _userSession.StoreId,
                request.DateFrom,
                request.DateTo,
                ct);

            return new GetSalesByProductReportResponse(rows);
        }
    }
}
