using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, DeleteProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<DeleteProductHandler> _logger;

        #region constructor
        public DeleteProductHandler(
            IStockRepository stockRepository,
            IUserSession userSession,
            IProductRepository productRepository,
            ILogger<DeleteProductHandler> logger)
        {
            _stockRepository = stockRepository;
            _userSession = userSession;
            _productRepository = productRepository;
            _logger = logger;
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
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }
                #region

                var productId = new List<Guid> { product.Id };
                var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productId);

                var stock = stocks.FirstOrDefault();
                if (stock == null || stock.StoreId != storeId)
                {
                    var message = "Sản phẩm không tồn tại trong kho.";
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }

                if (stock.Quantity >= 0)
                {
                    var message = "Số lượng sản phẩm còn tồn kho nên không thể xóa.";
                    _logger.LogWarning(message);
                    return new DeleteProductResponse(false, message);
                }
                #endregion
                await _productRepository.DeleteAsync(request.Id);

                return new DeleteProductResponse(true, "Xóa sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm.");
                return new DeleteProductResponse(false, "Không thể xóa sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
