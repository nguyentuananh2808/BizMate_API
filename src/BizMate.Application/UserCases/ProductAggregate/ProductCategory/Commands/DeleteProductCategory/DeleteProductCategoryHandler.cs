using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using BizMate.Application.Resources;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.ProductCategoryAggregate.ProductCategory.Commands.DeleteProductCategory
{
    public class DeleteProductCategoryHandler : IRequestHandler<DeleteProductCategoryRequest, DeleteProductCategoryResponse>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteProductCategoryHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;

        #region constructor
        public DeleteProductCategoryHandler(
            IUserSession userSession,
            IProductCategoryRepository ProductCategoryRepository,
            ILogger<DeleteProductCategoryHandler> logger,
            IStringLocalizer<MessageUtils> localizer)
        {
            _userSession = userSession;
            _productCategoryRepository = ProductCategoryRepository;
            _logger = logger;
            _localizer = localizer;
        }
        #endregion
        public async Task<DeleteProductCategoryResponse> Handle(DeleteProductCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var ProductCategory = await _productCategoryRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (ProductCategory == null || ProductCategory.StoreId != storeId)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new DeleteProductCategoryResponse(false, message);
                }

                var isUsedInInventory = await _productCategoryRepository.IsUsedInProduct(request.Id);

                if (isUsedInInventory)
                {
                    var message = ValidationMessage.LocalizedStrings.RecordBeingUsed;
                    _logger.LogWarning(message, request.Id);
                    return new DeleteProductCategoryResponse(false, message);
                }

                await _productCategoryRepository.DeleteAsync(request.Id, cancellationToken);

                return new DeleteProductCategoryResponse(true, _localizer["Xóa loại sản phẩm thành công."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa loại sản phẩm.");
                return new DeleteProductCategoryResponse(false, _localizer["Không thể xóa loại sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
