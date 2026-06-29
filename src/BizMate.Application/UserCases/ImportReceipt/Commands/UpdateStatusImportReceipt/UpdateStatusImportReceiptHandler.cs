using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptHandler
        : IRequestHandler<UpdateStatusImportReceiptRequest, UpdateStatusImportReceiptResponse>
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusImportReceiptHandler> _logger;

        public UpdateStatusImportReceiptHandler(
            IStockRepository stockRepository,
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IStatusRepository statusRepository,
            IUserSession userSession,
            IUnitOfWork unitOfWork,
            IImportReceiptRepository importReceiptRepository,
            ILogger<UpdateStatusImportReceiptHandler> logger)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _statusRepository = statusRepository;
            _userSession = userSession;
            _unitOfWork = unitOfWork;
            _importReceiptRepository = importReceiptRepository;
            _logger = logger;
        }

        public async Task<UpdateStatusImportReceiptResponse> Handle(
            UpdateStatusImportReceiptRequest request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                if (request.CodeStatus == "APPROVED" || request.CodeStatus != "APPROVED")
                {
                    return await RollbackResponseAsync(
                        "Phiếu nhập kho đã được cộng tồn ngay khi tạo mới, không còn bước duyệt nhập kho.",
                        cancellationToken);
                }

                var storeId = _userSession.StoreId;
                var userId = Guid.Parse(_userSession.UserId);
                var role = _userSession.Role;

                var importReceipt = await _importReceiptRepository.GetByIdAsync(request.Id, cancellationToken);
                if (importReceipt == null)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist, cancellationToken);

                if (importReceipt.RowVersion != request.RowVersion)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.NotValidRowversion, cancellationToken);

                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(request.CodeStatus, "ImportReceipt");
                if (statusId == Guid.Empty)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist2, cancellationToken);

                if (role == "Staff" && request.CodeStatus == "APPROVED")
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.RoleWithoutAuthority, cancellationToken);

                var shouldImportStock = request.CodeStatus == "APPROVED"
                    && importReceipt.Status?.Code != "APPROVED";

                if (shouldImportStock && importReceipt.Details.Any())
                {
                    var serialValidationMessage = await ValidatePendingSerialItemsAsync(
                        importReceipt.Details,
                        cancellationToken);

                    if (serialValidationMessage is not null)
                        return await RollbackResponseAsync(serialValidationMessage, cancellationToken);
                }

                var statusImportReceipt = new UpdateImportReceiptStatusDto
                {
                    Id = request.Id,
                    RowVersion = Guid.NewGuid(),
                    StatusId = statusId,
                    StoreId = storeId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                };

                await _importReceiptRepository.UpdateStatusAsync(statusImportReceipt, cancellationToken);

                if (shouldImportStock && importReceipt.Details.Any())
                {
                    await ImportStockAsync(importReceipt.Details, storeId, userId, cancellationToken);
                    await ActivatePendingSerialItemsAsync(importReceipt, userId, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new UpdateStatusImportReceiptResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái phiếu nhập kho.");
                return new UpdateStatusImportReceiptResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }

        private async Task<UpdateStatusImportReceiptResponse> RollbackResponseAsync(
            string message,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogWarning(message);
            return new UpdateStatusImportReceiptResponse(false, message);
        }

        private async Task ImportStockAsync(
            IEnumerable<ImportReceiptDetail> receiptDetails,
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var details = receiptDetails.ToList();
            var productIds = details.Select(d => d.ProductId).Distinct().ToList();

            var existingStocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = existingStocks.ToDictionary(s => s.ProductId, s => s);
            var stocksToUpdate = new List<Stock>();

            foreach (var detail in details)
            {
                if (stockDict.TryGetValue(detail.ProductId, out var stock))
                {
                    stock.Quantity += detail.Quantity;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;
                    stocksToUpdate.Add(stock);
                    continue;
                }

                var newStock = new Stock
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                };

                await _stockRepository.AddAsync(newStock);
                stockDict[detail.ProductId] = newStock;
            }

            if (stocksToUpdate.Count > 0)
                await _stockRepository.UpdateAsync(stocksToUpdate, cancellationToken);
        }

        private async Task<string?> ValidatePendingSerialItemsAsync(
            IEnumerable<ImportReceiptDetail> receiptDetails,
            CancellationToken cancellationToken)
        {
            var details = receiptDetails.ToList();
            var productIds = details.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
            var productDict = products.ToDictionary(x => x.Id);

            var pendingItems = await _productItemRepository.GetByImportReceiptDetailIdsAsync(
                details.Select(x => x.Id),
                ProductItemStatus.PendingImport,
                cancellationToken);

            var itemsByDetailId = pendingItems
                .Where(x => x.ImportReceiptDetailId.HasValue)
                .GroupBy(x => x.ImportReceiptDetailId!.Value)
                .ToDictionary(x => x.Key, x => x.Count());

            foreach (var detail in details)
            {
                if (!productDict.TryGetValue(detail.ProductId, out var product))
                    return $"San pham {detail.ProductId} khong ton tai.";

                if (!product.IsSerialTracked)
                    continue;

                var serialCount = itemsByDetailId.GetValueOrDefault(detail.Id);
                if (serialCount != detail.Quantity)
                    return $"San pham {detail.ProductCode} can {detail.Quantity} SN de duyet nhap kho.";
            }

            return null;
        }

        private async Task ActivatePendingSerialItemsAsync(
            BizMate.Domain.Entities.ImportReceipt importReceipt,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var items = await _productItemRepository.GetByImportReceiptDetailIdsAsync(
                importReceipt.Details.Select(x => x.Id),
                ProductItemStatus.PendingImport,
                cancellationToken);

            if (items.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var transactions = new List<InventoryTransaction>();

            foreach (var item in items)
            {
                var fromStatus = item.Status;
                item.Status = ProductItemStatus.InStock;
                item.UpdatedBy = userId;
                item.UpdatedDate = now;

                transactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductItemId = item.Id,
                    Type = InventoryTransactionType.Import,
                    FromStatus = fromStatus,
                    ToStatus = ProductItemStatus.InStock,
                    CreatedBy = userId,
                    CreatedDate = now,
                    Note = $"Import receipt {importReceipt.Code}"
                });
            }

            await _productItemRepository.UpdateRangeAsync(items, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }
    }
}
