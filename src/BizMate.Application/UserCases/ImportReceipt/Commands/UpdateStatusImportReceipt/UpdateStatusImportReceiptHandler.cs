using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptHandler
        : IRequestHandler<UpdateStatusImportReceiptRequest, UpdateStatusImportReceiptResponse>
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IAppMessageService _messageService;
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateStatusImportReceiptHandler> _logger;

        #region constructor
        public UpdateStatusImportReceiptHandler(
            IStockRepository stockRepository,
            IStatusRepository statusRepository,
            IAppMessageService messageService,
            IUserSession userSession,
            IImportReceiptRepository importReceiptRepository,
            ILogger<UpdateStatusImportReceiptHandler> logger)
        {
            _stockRepository = stockRepository;
            _statusRepository = statusRepository;
            _messageService = messageService;
            _userSession = userSession;
            _importReceiptRepository = importReceiptRepository;
            _logger = logger;
        }
        #endregion

        public async Task<UpdateStatusImportReceiptResponse> Handle(UpdateStatusImportReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = Guid.Parse(_userSession.UserId);
                var role = _userSession.Role;

                #region check importReceipt exist
                var importReceipt = await _importReceiptRepository.GetByIdAsync(request.Id);
                if (importReceipt == null)
                {
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Phiếu nhập kho không tồn tại.");
                }
                #endregion

                #region check rowversion
                if (importReceipt.RowVersion != request.RowVersion)
                {
                    return new UpdateStatusImportReceiptResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
                }
                #endregion

                #region get status
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(request.Code, "ImportReceipt");
                if (statusId == Guid.Empty)
                {
                    var message = _messageService.NotExist(request.Code);
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Trạng thái không tồn tại.");
                }
                #endregion

                #region check role
                if (role == "Staff" && request.Code == "APPROVED")
                {
                    var message = _messageService.Forbidden();
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Bạn không có quyền duyệt phiếu nhập kho.");
                }
                #endregion

                #region update status
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
                #endregion

                #region import stock if APPROVED
                if (request.Code == "APPROVED" && importReceipt.Details.Any())
                {
                    await ImportStockAsync(importReceipt.Details, storeId, userId, cancellationToken);
                }
                #endregion

                return new UpdateStatusImportReceiptResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái phiếu nhập kho.");
                return new UpdateStatusImportReceiptResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Cập nhật tồn kho theo phiếu nhập
        /// </summary>
        private async Task ImportStockAsync(
            IEnumerable<ImportReceiptDetail> receiptDetails,
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // 1. Lấy danh sách ProductId
            var productIds = receiptDetails.Select(d => d.ProductId).Distinct().ToList();

            // 2. Lấy toàn bộ tồn kho hiện có cho các sản phẩm
            var existingStocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = existingStocks.ToDictionary(s => s.ProductId, s => s);

            // 3. Xử lý từng chi tiết phiếu nhập
            foreach (var detail in receiptDetails)
            {
                if (stockDict.TryGetValue(detail.ProductId, out var stock))
                {
                    // Đã có tồn kho → update
                    stock.Quantity += detail.Quantity;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;

                    await _stockRepository.UpdateAsync(stock, cancellationToken);
                }
                else
                {
                    // Chưa có tồn kho → tạo mới
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
            }
        }
    }
}
