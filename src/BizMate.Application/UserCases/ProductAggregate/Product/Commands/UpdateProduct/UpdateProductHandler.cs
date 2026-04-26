using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IStockRepository _stockRepository;

        #region constructor
        public UpdateProductHandler(
            IUserSession userSession,
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            IStockRepository stockRepository,
            QueryFactory db,
            ILogger<UpdateProductHandler> logger,
            IMapper mapper)
        {
            _userSession = userSession;
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _stockRepository = stockRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateProductResponse> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check product exist
                var product = await _productRepository.GetByIdAsync(request.Id);
                if (product == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (product.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check for duplicate product names in the same store and supplier (except yourself)
                var duplicateProducts = await _productRepository.SearchProducts(storeId, request.SupplierId, request.Name, _db);
                if (duplicateProducts.Any(p => p.Id != request.Id))
                {
                    var message = ValidationMessage.LocalizedStrings.DataDuplicate;
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Serial tracking guards
                if (product.IsSerialTracked && !request.IsSerialTracked)
                {
                    var (_, totalItems) = await _productItemRepository.GetByProductAsync(
                        storeId,
                        product.Id,
                        null,
                        pageIndex: 1,
                        pageSize: 1,
                        ct: cancellationToken);

                    if (totalItems > 0)
                    {
                        return new UpdateProductResponse(false, "Khong the tat quan ly serial khi san pham da co SN.");
                    }
                }

                if (!product.IsSerialTracked && request.IsSerialTracked)
                {
                    var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, new List<Guid> { product.Id });
                    var currentStock = stocks.FirstOrDefault();

                    if (currentStock is not null && currentStock.Quantity > 0)
                    {
                        return new UpdateProductResponse(false, "Chi co the bat quan ly serial khi ton kho hien tai bang 0.");
                    }
                }
                #endregion

                #region update data
                product.Name = request.Name.Trim();
                product.Unit = request.Unit;
                product.SalePrice = request.SalePrice;
                product.CostPrice = request.CostPrice;
                product.ImageUrl = request.ImageUrl;
                product.Description = request.Description;
                product.SupplierId = request.SupplierId;
                product.StoreId = storeId;
                product.UpdatedBy = Guid.Parse(userId);
                product.UpdatedDate = DateTime.UtcNow;
                product.IsActive = request.IsActive;
                product.IsSerialTracked = request.IsSerialTracked;
                product.RowVersion = Guid.NewGuid();

                await _productRepository.UpdateAsync(product);
                #endregion

                var productDto = _mapper.Map<ProductCoreDto>(product);

                return new UpdateProductResponse(productDto, true, "Cập nhật sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm.");
                return new UpdateProductResponse(false, "Không thể cập nhật sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
