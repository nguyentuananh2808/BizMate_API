using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Application.Resources;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory;

namespace BizMate.Application.UserCases.ProductCategoryAggregate.ProductCategory.Commands.DeleteProductCategory
{
    public class DeleteProductCategoryHandler : IRequestHandler<DeleteProductCategoryRequest, DeleteProductCategoryResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<DeleteProductCategoryHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;

        #region constructor
        public DeleteProductCategoryHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IProductCategoryRepository ProductCategoryRepository,
            QueryFactory db,
            ILogger<DeleteProductCategoryHandler> logger,
            IStringLocalizer<MessageUtils> localizer)
        {
            _messageService = messageService;
            _userSession = userSession;
            _productCategoryRepository = ProductCategoryRepository;
            _db = db;
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
                    var message = _messageService.NotExist(request.Id);
                    _logger.LogWarning(message);
                    return new DeleteProductCategoryResponse(false, message);
                }

                var isUsedInInventory = await _productCategoryRepository.IsUsedInProduct(request.Id);

                if (isUsedInInventory)
                {
                    var message = _localizer["Loại sản phẩm đang được sử dụng trong sản phẩm. Không thể xóa."];
                    _logger.LogWarning("Không thể xóa lại sản phẩm {ProductCategoryId} vì đang được sử dụng", request.Id);
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
