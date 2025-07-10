using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategory
{
    public class GetProductCategoryHandler : IRequestHandler<GetProductCategoryRequest, GetProductCategoryResponse>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductCategoryHandler> _logger;

        #region constructor
        public GetProductCategoryHandler(
            IUserSession userSession,
            IProductCategoryRepository productCategoryRepository,
            IMapper mapper,
            ILogger<GetProductCategoryHandler> logger)
        {
            _userSession = userSession;
            _productCategoryRepository = productCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        public async Task<GetProductCategoryResponse> Handle(GetProductCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var productCategory = await _productCategoryRepository.GetByIdAsync(storeId, request.Id, cancellationToken);

                var mappedProductCategory = _mapper.Map<ProductCategoryCoreDto>(productCategory);
                return new GetProductCategoryResponse(mappedProductCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn loại sản phẩm.");
                return new GetProductCategoryResponse(false, "Không thể tải loại sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
