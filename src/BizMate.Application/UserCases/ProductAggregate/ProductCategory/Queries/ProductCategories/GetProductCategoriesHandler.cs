using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Queries.ProductCategories
{
    public class GetProductCategoriesHandler : IRequestHandler<GetProductCategoriesRequest, GetProductCategoriesResponse>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductCategoriesHandler> _logger;

        #region constructor
        public GetProductCategoriesHandler(
            IProductCategoryRepository productCategoryRepository,
            IUserSession userSession,
            IMapper mapper,
            ILogger<GetProductCategoriesHandler> logger)
        {
            _productCategoryRepository = productCategoryRepository;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetProductCategoriesResponse> Handle(GetProductCategoriesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (productCategories, totalCount) = await _productCategoryRepository.GetAllAsync(storeId, cancellationToken);
        
                var mappedProductCategories = _mapper.Map<List<ProductCategoryCoreDto>>(productCategories);

                return new GetProductCategoriesResponse(mappedProductCategories, totalCount,true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách sản phẩm.");
                return new GetProductCategoriesResponse(false, "Không thể tải danh sách sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
