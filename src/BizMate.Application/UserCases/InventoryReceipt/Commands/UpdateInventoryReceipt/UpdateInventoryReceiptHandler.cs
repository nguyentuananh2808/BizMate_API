using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using BizMate.Application.Resources;
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
            try
            {
                await _inventoryReceiptRepository.BeginTransactionAsync();

                var storeId = _userSession.StoreId;

                var existingReceipt = await _inventoryReceiptRepository.GetByIdAsync(request.Id);
                if (existingReceipt == null)
                {
                    var message = _messageService.NotExist(request.Id, _localizer);
                    _logger.LogWarning(message);
                    return new UpdateInventoryReceiptResponse(false, message);
                }

                // Trả lại tồn kho cũ
                foreach (var oldDetail in existingReceipt.Details)
                {
                    var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, oldDetail.ProductId);
                    if (stock != null)
                    {
                        stock.Quantity += existingReceipt.Type == 1 ? -oldDetail.Quantity : oldDetail.Quantity;
                        stock.UpdatedDate = DateTime.UtcNow;
                        await _stockRepository.UpdateAsync(stock);
                    }
                }

                // Cập nhật thông tin mới
                existingReceipt.SupplierName = request.SupplierName;
                existingReceipt.CustomerName = request.CustomerName;
                existingReceipt.CustomerPhone = request.CustomerPhone;
                existingReceipt.DeliveryAddress = request.DeliveryAddress;
                existingReceipt.Description = request.Description;
                existingReceipt.Date = DateTime.UtcNow;

                existingReceipt.Details.Clear();
                foreach (var d in request.Details)
                {
                    existingReceipt.Details.Add(new InventoryReceiptDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = d.ProductId,
                        Quantity = d.Quantity
                    });
                }

                await _inventoryReceiptRepository.UpdateAsync(existingReceipt);

                // Cập nhật lại tồn kho mới
                foreach (var detail in existingReceipt.Details)
                {
                    var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, detail.ProductId);
                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            Id = Guid.NewGuid(),
                            StoreId = storeId,
                            ProductId = detail.ProductId,
                            Quantity = 0,
                            UpdatedDate = DateTime.UtcNow
                        };
                        await _stockRepository.AddAsync(stock);
                    }

                    if (existingReceipt.Type == 1)
                        stock.Quantity += detail.Quantity;
                    else
                    {
                        if (stock.Quantity < detail.Quantity)
                            throw new InvalidOperationException($"Tồn kho sản phẩm không đủ: {detail.ProductId}");
                        stock.Quantity -= detail.Quantity;
                    }

                    stock.UpdatedDate = DateTime.UtcNow;
                    await _stockRepository.UpdateAsync(stock);
                }

                await _inventoryReceiptRepository.CommitTransactionAsync();

                var response = _mapper.Map<UpdateInventoryReceiptResponse>(existingReceipt);
                response.Success = true;
                response.Message = _localizer["Cập nhật sản phẩm thành công."];
                return response;
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
    }
}
