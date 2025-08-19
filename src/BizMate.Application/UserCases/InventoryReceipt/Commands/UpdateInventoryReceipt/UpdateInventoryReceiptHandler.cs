//using AutoMapper;
//using BizMate.Application.Common.Interfaces.Repositories;
//using BizMate.Application.Common.Security;
//using BizMate.Domain.Entities;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using BizMate.Application.Common.Interfaces;
//using _InventoryReceipt = BizMate.Domain.Entities.InventoryReceipt;
//using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
//using Microsoft.EntityFrameworkCore;

//namespace BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt
//{
//    public class UpdateInventoryReceiptHandler : IRequestHandler<UpdateInventoryReceiptRequest, UpdateInventoryReceiptResponse>
//    {
//        private readonly IAppMessageService _messageService;
//        private readonly IInventoryReceiptRepository _inventoryReceiptRepository;
//        private readonly IStockRepository _stockRepository;
//        private readonly IUserSession _userSession;
//        private readonly IMapper _mapper;
//        private readonly ILogger<UpdateInventoryReceiptHandler> _logger;
//        private readonly IProductRepository _productRepository;

//        public UpdateInventoryReceiptHandler(
//            IAppMessageService messageService,
//            IInventoryReceiptRepository inventoryReceiptRepository,
//            IStockRepository stockRepository,
//            IUserSession userSession,
//            IMapper mapper,
//            ILogger<UpdateInventoryReceiptHandler> logger,
//            IProductRepository productRepository)
//        {
//            _messageService = messageService;
//            _inventoryReceiptRepository = inventoryReceiptRepository;
//            _stockRepository = stockRepository;
//            _userSession = userSession;
//            _mapper = mapper;
//            _logger = logger;
//            _productRepository = productRepository;
//        }

//        public async Task<UpdateInventoryReceiptResponse> Handle(UpdateInventoryReceiptRequest request, CancellationToken cancellationToken)
//        {
//            await _inventoryReceiptRepository.BeginTransactionAsync();

//            try
//            {
//                var storeId = _userSession.StoreId;

//                var existingReceipt = await _inventoryReceiptRepository.GetByIdAsync(request.Id);
//                if (existingReceipt == null)
//                {
//                    var message = _messageService.NotExist(request.Id);
//                    _logger.LogWarning(message);
//                    return new UpdateInventoryReceiptResponse(false, message);
//                }

//                if (existingReceipt.RowVersion != request.RowVersion)
//                {
//                    return new UpdateInventoryReceiptResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
//                }

//                var validationMessage = await ValidateStockBeforeUpdateAsync(existingReceipt, request.Details, storeId);
//                if (validationMessage != null)
//                {
//                    return new UpdateInventoryReceiptResponse(false, validationMessage);
//                }

//                // Hoàn tồn kho cũ
//                foreach (var oldDetail in existingReceipt.Details.ToList())
//                {
//                    await RevertStockAsync(storeId, oldDetail.ProductId, oldDetail.Quantity, existingReceipt.Type);
//                }

//                // Cập nhật phiếu
//                existingReceipt.SupplierName = request.SupplierName;
//                existingReceipt.CustomerName = request.CustomerName;
//                existingReceipt.CustomerPhone = request.CustomerPhone;
//                existingReceipt.DeliveryAddress = request.DeliveryAddress;
//                existingReceipt.Description = request.Description;
//                existingReceipt.Date = DateTime.UtcNow;
//                existingReceipt.UpdatedDate = DateTime.UtcNow;
//                existingReceipt.RowVersion = Guid.NewGuid();

//                // Xóa và thêm lại chi tiết
               

//                //// Thêm lại chi tiết:
//                //foreach (var item in request.Details)
//                //{
//                //    var product = await _productRepository.GetByIdAsync(item.ProductId);
//                //    existingReceipt.Details.Add(new InventoryReceiptDetail
//                //    {
//                //        Id = Guid.NewGuid(),
//                //        ProductId = item.ProductId,
//                //        Quantity = item.Quantity,
//                //        InventoryReceiptId = existingReceipt.Id,
//                //        ProductName = product.Name,
//                //        ProductCode = product.Code,
//                //        Unit = product.Unit
//                //    });
//                //}


//                await _inventoryReceiptRepository.UpdateAsync(existingReceipt);

//                // Cập nhật tồn kho mới
//                foreach (var detail in existingReceipt.Details)
//                {
//                    await ApplyStockChangeAsync(storeId, detail.ProductId, detail.Quantity, existingReceipt.Type);
//                }

//                await _inventoryReceiptRepository.CommitTransactionAsync();

//                var response = _mapper.Map<UpdateInventoryReceiptResponse>(existingReceipt);
//                response.Success = true;
//                response.Message = "Cập nhật phiếu thành công.";
//                return response;
//            }
//            catch (Exception ex)
//            {
//                await _inventoryReceiptRepository.RollbackTransactionAsync();
//                _logger.LogError(ex, "Lỗi khi cập nhật phiếu.");
//                return new UpdateInventoryReceiptResponse(false, "Không thể cập nhật phiếu. Vui lòng thử lại.");
//            }
//        }

//        private async Task<string?> ValidateStockBeforeUpdateAsync(
//            _InventoryReceipt existingReceipt,
//            List<InventoryReceiptDetailRequestDto> newDetails,
//            Guid storeId)
//        {
//            var stockMap = new Dictionary<Guid, int>();

//            foreach (var oldDetail in existingReceipt.Details)
//            {
//                int delta = existingReceipt.Type == 1 ? -oldDetail.Quantity : oldDetail.Quantity;
//                stockMap[oldDetail.ProductId] = stockMap.GetValueOrDefault(oldDetail.ProductId) + delta;
//            }

//            foreach (var newDetail in newDetails)
//            {
//                int delta = existingReceipt.Type == 1 ? newDetail.Quantity : -newDetail.Quantity;
//                stockMap[newDetail.ProductId] = stockMap.GetValueOrDefault(newDetail.ProductId) + delta;
//            }

//            foreach (var kvp in stockMap)
//            {
//                var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, kvp.Key);
//                var currentQty = stock?.Quantity ?? 0;
//                var newQty = currentQty + kvp.Value;

//                if (newQty < 0)
//                {
//                    return $"Không thể cập nhật phiếu. Sản phẩm ID {kvp.Key} không đủ tồn kho.";
//                }
//            }

//            return null;
//        }

//        private async Task RevertStockAsync(Guid storeId, Guid productId, int quantity, int type)
//        {
//            var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, productId);
//            if (stock != null)
//            {
//                stock.Quantity += type == 1 ? -quantity : quantity;
//                stock.UpdatedDate = DateTime.UtcNow;
//                await _stockRepository.UpdateAsync(stock);
//            }
//        }

//        private async Task ApplyStockChangeAsync(Guid storeId, Guid productId, int quantity, int type)
//        {
//            var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, productId);
//            if (stock == null)
//            {
//                stock = new Stock
//                {
//                    Id = Guid.NewGuid(),
//                    StoreId = storeId,
//                    ProductId = productId,
//                    Quantity = 0,
//                    UpdatedDate = DateTime.UtcNow,
//                    RowVersion = Guid.NewGuid()
//                };
//                await _stockRepository.AddAsync(stock);
//            }

//            if (type == 1)
//            {
//                stock.Quantity += quantity;
//            }
//            else
//            {
//                if (stock.Quantity < quantity)
//                    throw new InvalidOperationException($"Tồn kho không đủ để xuất: {productId}");

//                stock.Quantity -= quantity;
//            }

//            stock.UpdatedDate = DateTime.UtcNow;
//            await _stockRepository.UpdateAsync(stock);
//        }
//    }
//}