using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using _Product = BizMate.Domain.Entities.Product;

namespace BizMate.Application.UserCases.Product.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly IStringLocalizer<UpdateProductHandler> _localizer;
        private readonly IMapper _mapper;

        public UpdateProductHandler(
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<UpdateProductHandler> logger,
            IStringLocalizer<UpdateProductHandler> localizer,
            IMapper mapper)
        {
            _userSession = userSession;
            _productRepository = productRepository;
            _db = db;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<UpdateProductResponse> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                #region check product exist
                var product = await _productRepository.GetByIdAsync(request.Id);
                if (product == null)
                {
                    var message = CommonAppMessageUtils.NotExist<_Product>(request.Id.ToString(), _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (product.RowVersion != request.RowVersion)
                {
                    var message = CommonAppMessageUtils.ConcurrencyConflict(_localizer);
                    _logger.LogWarning("RowVersion conflict: Request={RequestVersion}, DB={DbVersion}", request.RowVersion, product.RowVersion);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check for duplicate product names in the same store and supplier (except yourself)
                var duplicateProducts = await _productRepository.SearchProducts(storeId, request.SupplierId, request.Name, _db);
                if (duplicateProducts.Any(p => p.Id != request.Id))
                {
                    var message = CommonAppMessageUtils.DuplicateData(request.Name, _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region update data
                product.Name = request.Name;
                product.Quantity = request.Quantity;
                product.Unit = request.Unit;
                product.ImageUrl = request.ImageUrl;
                product.Description = request.Description;
                product.SupplierId = request.SupplierId;
                product.StoreId = storeId;
                product.RowVersion += 1;

                await _productRepository.UpdateAsync(product);
                #endregion

                var productDto = _mapper.Map<ProductCoreDto>(product);

                return new UpdateProductResponse(productDto, true, _localizer["Cập nhật sản phẩm thành công."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm.");
                return new UpdateProductResponse(false, _localizer["Không thể cập nhật sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
