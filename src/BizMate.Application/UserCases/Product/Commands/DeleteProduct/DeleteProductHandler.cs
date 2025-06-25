using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using _Product = BizMate.Domain.Entities.Product;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Product.Commands.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, DeleteProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<DeleteProductHandler> _logger;
        private readonly IStringLocalizer<DeleteProductHandler> _localizer;

        #region constructor
        public DeleteProductHandler(
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<DeleteProductHandler> logger,
            IStringLocalizer<DeleteProductHandler> localizer)
        {
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
                    var message = CommonAppMessageUtils.NotExist<_Product>(request.Id, _localizer);
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }

                var isUsedInInventory = await _db.Query("InventoryReceiptDetails")
                    .Where("ProductId", request.Id)
                    .ExistsAsync();

                if (isUsedInInventory)
                {
                    var message = _localizer["Sản phẩm đang được sử dụng trong phiếu nhập kho. Không thể xóa."];
                    _logger.LogWarning("Không thể xóa sản phẩm {ProductId} vì đang được sử dụng", request.Id);
                    return new DeleteProductResponse(false, message);
                }

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
