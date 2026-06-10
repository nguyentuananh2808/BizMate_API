using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;

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
        public int Quantity { get; set; }
    }

    public class UseBorrowedItemRequest : IRequest<BorrowingMutationResponse>
    {
        public Guid OrderId { get; set; }
        public Guid? TechnicianId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
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
        public int Quantity { get; set; }
        public DateTime LastBorrowedAt { get; set; }
        public bool IsOverdue { get; set; }
        public string? ReminderMessage { get; set; }
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

        public ExportOrderForTechnicianHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _repo = repo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<BorrowingMutationResponse> Handle(
            ExportOrderForTechnicianRequest request,
            CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);

            try
            {
                if (request.OrderId == Guid.Empty)
                    return await RollbackAsync("OrderId khong hop le.", ct);

                var technicianExports = NormalizeTechnicianExports(request);
                if (technicianExports.Count == 0)
                    return await RollbackAsync("Can co it nhat mot ky thuat va san pham de xuat hang.", ct);
                if (technicianExports.Any(x => x.TechnicianId == Guid.Empty))
                    return await RollbackAsync("TechnicianId khong hop le.", ct);
                if (technicianExports.GroupBy(x => x.TechnicianId).Any(x => x.Count() > 1))
                    return await RollbackAsync("Payload xuat hang bi trung ky thuat.", ct);
                if (technicianExports.Any(x => x.Items.Count == 0))
                    return await RollbackAsync("Moi ky thuat can co it nhat mot san pham de xuat hang.", ct);
                if (technicianExports
                    .SelectMany(x => x.Items)
                    .Any(x => x.ProductId == Guid.Empty
                        || x.OrderedQuantity < 0
                        || x.BorrowedQuantity < 0
                        || (x.OrderedQuantity == 0 && x.BorrowedQuantity == 0)))
                {
                    return await RollbackAsync("Du lieu san pham xuat hang khong hop le.", ct);
                }
                if (technicianExports.Any(x => x.Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1)))
                    return await RollbackAsync("Payload xuat hang bi trung san pham trong cung ky thuat.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var order = await _repo.GetOrderWithDetailsAsync(request.OrderId, storeId, ct);
                if (order is null)
                    return await RollbackAsync("Khong tim thay don hang trong store hien tai.", ct);
                if (order.TechnicianExportedAt.HasValue)
                    return await RollbackAsync("Don hang da duoc xuat cho ky thuat.", ct);

                var technicianIds = technicianExports.Select(x => x.TechnicianId).ToList();
                var technicians = await _repo.GetTechniciansByIdsAsync(storeId, technicianIds, ct);
                if (technicians.Count != technicianIds.Count || technicians.Any(x => !x.IsActive))
                    return await RollbackAsync("Co ky thuat khong ton tai hoac dang ngung hoat dong trong store hien tai.", ct);

                var details = order.Details.Where(x => !x.IsDeleted).ToList();
                var duplicateProducts = details.GroupBy(x => x.ProductId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (duplicateProducts.Count > 0)
                    return await RollbackAsync("Flow muon hang yeu cau moi san pham chi co mot dong trong don.", ct);

                var detailByProduct = details.ToDictionary(x => x.ProductId);
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
                    return await RollbackAsync("San pham xuat ban phai nam trong don hang.", ct);

                foreach (var detail in detailByProduct.Values)
                {
                    if (!aggregateByProduct.TryGetValue(detail.ProductId, out var aggregate)
                        || aggregate.OrderedQuantity != detail.Quantity)
                    {
                        return await RollbackAsync(
                            $"Tổng số lượng bán của sản phẩm {detail.ProductName} phải bằng số lượng trong đơn.",
                            ct);
                    }
                }

                var stocks = await _repo.GetStocksAsync(storeId, aggregateByProduct.Keys, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var aggregate in aggregateByProduct.Values)
                {
                    detailByProduct.TryGetValue(aggregate.ProductId, out var detail);
                    var productCode = detail?.ProductCode ?? aggregate.ProductId.ToString();

                    if (!stockByProduct.TryGetValue(aggregate.ProductId, out var stock))
                        return await RollbackAsync($"Khong tim thay ton kho cho san pham {productCode}.", ct);

                    productCode = stock.Product.Code;
                    if (stock.Product.IsSerialTracked)
                        return await RollbackAsync(
                            $"Flow xuất/mượn cho kỹ thuật chưa hỗ trợ sản phẩm quản lý serial: {stock.Product.Name}.",
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
                        return await RollbackAsync($"San pham {productCode} khong du ton kha dung de cho muon.", ct);

                    stock.Quantity -= totalExportQuantity;
                    stock.Reserved -= reservedToRelease;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = now;

                    if (detail is not null)
                    {
                        detail.BorrowedQuantity = aggregate.BorrowedQuantity;
                        detail.UpdatedBy = userId;
                        detail.UpdatedDate = now;
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
                                CreatedDate = now
                            };
                            _repo.AddHolding(holding);
                        }
                        else
                        {
                            holding.Quantity += item.BorrowedQuantity;
                            holding.LastBorrowedAt = now;
                            holding.UpdatedBy = userId;
                            holding.UpdatedDate = now;
                        }

                        _repo.AddTransaction(NewTransaction(
                            storeId,
                            technicianExport.TechnicianId,
                            item.ProductId,
                            HoldingTransactionType.Borrow,
                            item.BorrowedQuantity,
                            "order",
                            order.Id,
                            detailByProduct.ContainsKey(item.ProductId)
                                ? "Borrowed with order export"
                                : "Borrowed extra product with order export",
                            userId,
                            now));
                    }
                }

                AddMissingOrderTechnicians(order, technicianIds, now);
                order.TechnicianId ??= technicianIds.First();
                order.TechnicianExportedAt = now;
                order.UpdatedBy = userId;
                order.UpdatedDate = now;

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);
                return new BorrowingMutationResponse(true, "Xuat hang va ghi nhan hang muon thanh cong.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync(ct);
                return new BorrowingMutationResponse(false, ex.Message);
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
                    return await RollbackAsync("TechnicianId khong hop le.", ct);
                if (request.Items.Count == 0 || request.Items.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0))
                    return await RollbackAsync("Du lieu tra hang khong hop le.", ct);
                if (request.Items.GroupBy(x => x.ProductId).Any(x => x.Count() > 1))
                    return await RollbackAsync("Payload tra hang bi trung san pham.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var technician = await _repo.GetTechnicianAsync(request.TechnicianId, storeId, ct);
                if (technician is null)
                    return await RollbackAsync("Khong tim thay ky thuat trong store hien tai.", ct);

                var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
                var stocks = await _repo.GetStocksAsync(storeId, productIds, ct);
                var stockByProduct = stocks.ToDictionary(x => x.ProductId);

                foreach (var item in request.Items)
                {
                    var holding = await _repo.GetHoldingAsync(storeId, technician.Id, item.ProductId, ct);
                    if (holding is null || holding.Quantity < item.Quantity)
                        return await RollbackAsync("So luong tra vuot qua so luong ky thuat dang giu.", ct);
                    if (!stockByProduct.TryGetValue(item.ProductId, out var stock))
                        return await RollbackAsync("Khong tim thay ton kho de nhap lai hang tra.", ct);

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
                return new BorrowingMutationResponse(true, "Tra hang thanh cong.");
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
                    return await RollbackAsync("Du lieu dung hang muon khong hop le.", ct);

                var storeId = _userSession.StoreId;
                var userId = CurrentUserId(_userSession);
                var now = DateTime.UtcNow;

                var order = await _repo.GetOrderWithDetailsAsync(request.OrderId, storeId, ct);
                if (order is null)
                    return await RollbackAsync("Khong tim thay don hang trong store hien tai.", ct);

                var productHoldings = await _repo.GetHoldingsByProductAsync(storeId, request.ProductId, ct);
                var technicianId = ResolveTechnicianId(order, request.TechnicianId, productHoldings);
                if (!technicianId.HasValue)
                    return await RollbackAsync("Can chon ky thuat dung hang muon cho don co nhieu ky thuat.", ct);

                var detail = order.Details.FirstOrDefault(x => x.ProductId == request.ProductId && !x.IsDeleted);
                if (detail is null)
                    return await RollbackAsync("San pham khong thuoc don hang.", ct);

                var remainingBorrowed = detail.BorrowedQuantity - detail.UsedBorrowedQuantity;
                if (remainingBorrowed < request.Quantity)
                    return await RollbackAsync("So luong dung vuot qua so luong da muon con lai cua don.", ct);

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
                return new BorrowingMutationResponse(true, "Da chuyen hang muon thanh hang ban.");
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
            var holdings = await _repo.GetHoldingsAsync(_userSession.StoreId, request.TechnicianId, ct);
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
                        var isOverdue = x.Quantity > 0 && x.LastBorrowedAt <= overdueBefore;
                        return new TechnicianHoldingItemDto
                        {
                            ProductId = x.ProductId,
                            ProductName = x.Product.Name,
                            ProductCode = x.Product.Code,
                            Quantity = x.Quantity,
                            LastBorrowedAt = x.LastBorrowedAt,
                            IsOverdue = isOverdue,
                            ReminderMessage = isOverdue
                                ? $"Ky thuat {x.Technician.Name} con giu {x.Quantity} {x.Product.Name}, vui long tra hang cuoi ngay."
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
