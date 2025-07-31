using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using BizMate.Application.Resources;
using Microsoft.EntityFrameworkCore;
using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt
{
    public class UpdateInventoryReceiptHandler : IRequestHandler<UpdateInventoryReceiptRequest, UpdateInventoryReceiptResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IInventoryReceiptRepository _inventoryReceiptRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateInventoryReceiptHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;

        public UpdateInventoryReceiptHandler(
            IAppMessageService messageService,
            IInventoryReceiptRepository inventoryReceiptRepository,
            IStockRepository stockRepository,
            IUserSession userSession,
            IMapper mapper,
            ILogger<UpdateInventoryReceiptHandler> logger,
            IStringLocalizer<MessageUtils> localizer)
        {
            _messageService = messageService;
            _inventoryReceiptRepository = inventoryReceiptRepository;
            _stockRepository = stockRepository;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<UpdateInventoryReceiptResponse> Handle(UpdateInventoryReceiptRequest request, CancellationToken cancellationToken)
        {
            await _inventoryReceiptRepository.BeginTransactionAsync();

            try
            {
                var storeId = _userSession.StoreId;

                // 1. Lấy phiếu hiện tại
                var existingReceipt = await _inventoryReceiptRepository.GetByIdAsync(request.Id);
                if (existingReceipt == null)
                {
                    var message = _messageService.NotExist(request.Id, _localizer);
                    _logger.LogWarning(message);
                    return new UpdateInventoryReceiptResponse(false, message);
                }

                // 2. Thiết lập OriginalValue của RowVersion để kiểm tra concurrency
                var entry = _inventoryReceiptRepository.Entry(existingReceipt);
                entry.Property("RowVersion").OriginalValue = request.RowVersion; 


                // 3. Trả lại tồn kho cũ
                foreach (var oldDetail in existingReceipt.Details.ToList())
                {
                    await RevertStockAsync(storeId, oldDetail.ProductId, oldDetail.Quantity, existingReceipt.Type);
                }

                // 4. Cập nhật thông tin chính của phiếu
                existingReceipt.SupplierName = request.SupplierName;
                existingReceipt.CustomerName = request.CustomerName;
                existingReceipt.CustomerPhone = request.CustomerPhone;
                existingReceipt.DeliveryAddress = request.DeliveryAddress;
                existingReceipt.Description = request.Description;
                existingReceipt.Date = DateTime.UtcNow;
                existingReceipt.UpdatedDate = DateTime.UtcNow;

                // 5. Cập nhật chi tiết phiếu (xóa - thêm - cập nhật)
                var requestDetails = request.Details;

                // Xóa những chi tiết không còn tồn tại
                var toRemove = existingReceipt.Details
                    .Where(d => !requestDetails.Any(x => x.ProductId == d.ProductId))
                    .ToList();
                foreach (var d in toRemove)
                {
                    existingReceipt.Details.Remove(d);
                }

                // Cập nhật hoặc thêm mới
                foreach (var item in requestDetails)
                {
                    var existing = existingReceipt.Details.FirstOrDefault(d => d.ProductId == item.ProductId);
                    if (existing != null)
                    {
                        existing.Quantity = item.Quantity;
                    }
                    else
                    {
                        existingReceipt.Details.Add(new InventoryReceiptDetail
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                    }
                }

                // 6. Lưu phiếu cập nhật
                await _inventoryReceiptRepository.UpdateAsync(existingReceipt);

                // 7. Cập nhật tồn kho mới
                foreach (var detail in existingReceipt.Details)
                {
                    await ApplyStockChangeAsync(storeId, detail.ProductId, detail.Quantity, existingReceipt.Type);
                }

                await _inventoryReceiptRepository.CommitTransactionAsync();

                // 8. Trả response
                var response = _mapper.Map<UpdateInventoryReceiptResponse>(existingReceipt);
                response.Success = true;
                response.Message = _localizer["Cập nhật phiếu thành công."];
                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await _inventoryReceiptRepository.RollbackTransactionAsync();
                var message = _localizer["Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại trước khi chỉnh sửa."];
                _logger.LogWarning(message);
                return new UpdateInventoryReceiptResponse(false, message);
            }
            catch (Exception ex)
            {
                await _inventoryReceiptRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi cập nhật phiếu.");
                return new UpdateInventoryReceiptResponse
                {
                    Success = false,
                    Message = _localizer["Không thể cập nhật phiếu. Vui lòng thử lại."]
                };
            }
        }

        private async Task RevertStockAsync(Guid storeId, Guid productId, int quantity, int type)
        {
            var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, productId);
            if (stock != null)
            {
                stock.Quantity += (type == 1) ? -quantity : quantity;
                stock.UpdatedDate = DateTime.UtcNow;
                await _stockRepository.UpdateAsync(stock);
            }
        }

        private async Task ApplyStockChangeAsync(Guid storeId, Guid productId, int quantity, int type)
        {
            var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, productId);
            if (stock == null)
            {
                stock = new Stock
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    ProductId = productId,
                    Quantity = 0,
                    UpdatedDate = DateTime.UtcNow
                };
                await _stockRepository.AddAsync(stock);
            }

            if (type == 1) // Nhập
            {
                stock.Quantity += quantity;
            }
            else // Xuất
            {
                if (stock.Quantity < quantity)
                    throw new InvalidOperationException($"Tồn kho sản phẩm không đủ để xuất: {productId}");

                stock.Quantity -= quantity;
            }

            stock.UpdatedDate = DateTime.UtcNow;
            await _stockRepository.UpdateAsync(stock);
        }
    }
}
