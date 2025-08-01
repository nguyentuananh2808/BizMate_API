using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.Resources;
using BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory
{
    public class UpdateProductCategoryHandler : IRequestHandler<UpdateProductCategoryRequest, UpdateProductCategoryResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateProductCategoryHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateProductCategoryHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IProductCategoryRepository productCategoryRepository,
            QueryFactory db,
            ILogger<UpdateProductCategoryHandler> logger,
            IStringLocalizer<MessageUtils> localizer,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _productCategoryRepository = productCategoryRepository;
            _db = db;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateProductCategoryResponse> Handle(UpdateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var name = request.Name.Trim();
                var description = request.Description?.Trim();
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check product category exist
                var productCategory = await _productCategoryRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (productCategory == null)
                {
                    var message = _messageService.NotExist(request.Id.ToString(), _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, message);
                }
                #endregion

                #region check duplicate name product category 
                var nameProductCategory = await _productCategoryRepository.IsNameProductCategoryAsync(storeId, name, request.Id, cancellationToken);
                if (nameProductCategory != null)
                {
                    var message = _messageService.NotExist(request.Name.ToString(), _localizer);
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, message);
                }
                #endregion

                //#region Check rowversion
                //if (productCategory.RowVersion != request.RowVersion)
                //{
                //    var message = _messageService.ConcurrencyConflict(_localizer);
                //    _logger.LogWarning("RowVersion conflict: Request={RequestVersion}, DB={DbVersion}", request.RowVersion, productCategory.RowVersion);
                //    return new UpdateProductCategoryResponse(false, message);
                //}
                //#endregion

                #region update data
                productCategory.Name = name;
                productCategory.Description = description;
                productCategory.StoreId = storeId;
                productCategory.UpdatedDate = DateTime.UtcNow;
                productCategory.UpdatedBy = Guid.Parse(userId);
                productCategory.IsActive = request.IsActive;


                await _productCategoryRepository.UpdateAsync(productCategory, cancellationToken);
                #endregion

                var productCategoryDto = _mapper.Map<ProductCategoryCoreDto>(productCategory);

                return new UpdateProductCategoryResponse(productCategoryDto, true, _localizer["Cập nhật loại sản phẩm thành công."]);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var msg = _messageService.ConcurrencyConflict(_localizer);
                _logger.LogWarning(ex, "DbUpdateConcurrencyException: Có xung đột khi cập nhật sản phẩm {ProductId}", request.Id);
                return new UpdateProductCategoryResponse(false, msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại sản phẩm.");
                return new UpdateProductCategoryResponse(false, _localizer["Không thể cập nhật loại sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
