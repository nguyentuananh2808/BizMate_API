using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Message;
using BizMate.Application.Common.Security;
using BizMate.Application.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System.Linq;
using _Product = BizMate.Domain.Entities.Product;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, UpdateProductResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateProductHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<UpdateProductHandler> logger,
            IStringLocalizer<MessageUtils> localizer,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _productRepository = productRepository;
            _db = db;
            _logger = logger;
            _localizer = localizer;
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
                    var message = _messageService.NotExist(request.Id.ToString(), _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (!product.RowVersion.SequenceEqual(request.RowVersion))
                {
                    var message = _messageService.ConcurrencyConflict(_localizer);
                    _logger.LogWarning("RowVersion conflict: Request={RequestVersion}, DB={DbVersion}",
                        Convert.ToBase64String(request.RowVersion),
                        Convert.ToBase64String(product.RowVersion));
                    return new UpdateProductResponse(false, message);
                }
                #endregion

                #region Check for duplicate product names in the same store and supplier (except yourself)
                var duplicateProducts = await _productRepository.SearchProducts(storeId, request.SupplierId, request.Name, _db);
                if (duplicateProducts.Any(p => p.Id != request.Id))
                {
                    var message = _messageService.DuplicateData(request.Name, _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductResponse(false, message);
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

                await _productRepository.UpdateAsync(product);
                #endregion

                var productDto = _mapper.Map<ProductCoreDto>(product);

                return new UpdateProductResponse(productDto, true, _localizer["Cập nhật sản phẩm thành công."]);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var msg = _messageService.ConcurrencyConflict(_localizer);
                _logger.LogWarning(ex, "DbUpdateConcurrencyException: Có xung đột khi cập nhật sản phẩm {ProductId}", request.Id);
                return new UpdateProductResponse(false, msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm.");
                return new UpdateProductResponse(false, _localizer["Không thể cập nhật sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
