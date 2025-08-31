using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory
{
    public class UpdateProductCategoryHandler : IRequestHandler<UpdateProductCategoryRequest, UpdateProductCategoryResponse>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateProductCategoryHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateProductCategoryHandler(
            IUserSession userSession,
            IProductCategoryRepository productCategoryRepository,
            ILogger<UpdateProductCategoryHandler> logger,
            IMapper mapper)
        {
            _userSession = userSession;
            _productCategoryRepository = productCategoryRepository;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateProductCategoryResponse> Handle(UpdateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var name = request.Name?.Trim();
                var description = request.Description?.Trim();
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check product category exist
                var productCategory = await _productCategoryRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (productCategory == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, message);
                }
                #endregion

                #region check duplicate name product category 
                var nameProductCategory = await _productCategoryRepository.IsNameProductCategoryAsync(storeId, name, request.Id, cancellationToken);
                if (nameProductCategory != null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataDuplicate;
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (productCategory.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, message);
                }
                #endregion

                #region update data
                productCategory.Name = name;
                productCategory.Description = description;
                productCategory.StoreId = storeId;
                productCategory.UpdatedDate = DateTime.UtcNow;
                productCategory.UpdatedBy = Guid.Parse(userId);
                productCategory.IsActive = request.IsActive;
                productCategory.RowVersion = Guid.NewGuid();

                await _productCategoryRepository.UpdateAsync(productCategory, cancellationToken);
                #endregion

                var productCategoryDto = _mapper.Map<ProductCategoryCoreDto>(productCategory);

                return new UpdateProductCategoryResponse(productCategoryDto, true, "Cập nhật loại sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật loại sản phẩm.");
                return new UpdateProductCategoryResponse(false, "Không thể cập nhật loại sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
