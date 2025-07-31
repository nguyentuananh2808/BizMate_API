using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Application.Resources;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt;
using BizMate.Domain.Entities;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, DeleteProductResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly IStockRepository _stockRepository;
        private readonly QueryFactory _db;
        private readonly ILogger<DeleteProductHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;

        #region constructor
        public DeleteProductHandler(
            IStockRepository stockRepository,
            IAppMessageService messageService,
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<DeleteProductHandler> logger,
            IStringLocalizer<MessageUtils> localizer)
        {
            _stockRepository = stockRepository;
            _messageService = messageService;
            _userSession = userSession;
            _productRepository = productRepository;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }
        #endregion
        public async Task<DeleteProductResponse> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var product = await _productRepository.GetByIdAsync(request.Id);
                if (product == null || product.StoreId != storeId)
                {
                    var message = _messageService.NotExist(request.Id, _localizer);
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }
                #region
                var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, product.Id);
                if (stock.Quantity >= 0)
                {
                    var message = "Số lượng sản phẩm còn tồn kho nên không thể xóa.";
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }
                #endregion
                /*var isUsedInInventory = await _db.Query("InventoryReceiptDetails")
                    .Where("ProductId", request.Id)
                    .ExistsAsync();

                if (isUsedInInventory)
                {
                    var message = _localizer["Sản phẩm đang được sử dụng trong phiếu nhập kho. Không thể xóa."];
                    _logger.LogWarning("Không thể xóa sản phẩm {ProductId} vì đang được sử dụng", request.Id);
                    return new DeleteProductResponse(false, message);
                }*/

                await _productRepository.DeleteAsync(request.Id);

                return new DeleteProductResponse(true, _localizer["Xóa sản phẩm thành công."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm.");
                return new DeleteProductResponse(false, _localizer["Không thể xóa sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
