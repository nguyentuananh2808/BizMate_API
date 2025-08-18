using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateProductHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<UpdateProductHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _productRepository = productRepository;
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
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, "Sản phẩm không tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (product.RowVersion != request.RowVersion)
                    return new UpdateProductResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
                #endregion

                #region Check for duplicate product names in the same store and supplier (except yourself)
                var duplicateProducts = await _productRepository.SearchProducts(storeId, request.SupplierId, request.Name, _db);
                if (duplicateProducts.Any(p => p.Id != request.Id))
                {
                    var message = _messageService.DuplicateData(request.Name);
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, $"Sản phẩm {request.Name} đã tồn tại.");
                }
                #endregion

                #region update data
                product.Name = request.Name;
                product.Unit = request.Unit;
                product.ImageUrl = request.ImageUrl;
                product.Description = request.Description;
                product.SupplierId = request.SupplierId;
                product.StoreId = storeId;
                product.UpdatedBy = Guid.Parse(userId);
                product.UpdatedDate = DateTime.UtcNow;
                product.IsActive = request.IsActive;
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
