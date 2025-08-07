using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;

        #region constructor
        public UpdateProductCategoryHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IProductCategoryRepository productCategoryRepository,
            QueryFactory db,
            ILogger<UpdateProductCategoryHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _productCategoryRepository = productCategoryRepository;
            _db = db;
            _logger = logger;
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
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, "Loại sản phẩm không tồn tại.");
                }
                #endregion

                #region check duplicate name product category 
                var nameProductCategory = await _productCategoryRepository.IsNameProductCategoryAsync(storeId, name, request.Id, cancellationToken);
                if (nameProductCategory != null)
                {
                    var message = _messageService.DuplicateData(request.Name.ToString());
                    _logger.LogWarning(message);
                    return new UpdateProductCategoryResponse(false, $"Loại sản phẩm {request.Name} đã tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (productCategory.RowVersion != request.RowVersion)
                    return new UpdateProductCategoryResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");

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
