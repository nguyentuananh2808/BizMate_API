using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class ProductsHandler : IRequestHandler<ProductsRequest, ProductsResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsHandler> _logger;

        public ProductsHandler(
            IProductRepository productRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<ProductsHandler> logger)
        {
            _productRepository = productRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductsResponse> Handle(ProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                // Lấy danh sách sản phẩm kèm phân trang
                var (products, totalCount) = await _productRepository.SearchProductsWithPaging(
                    storeId: storeId,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory);

                var mappedProducts = _mapper.Map<List<ProductCoreDto>>(products);
                return new ProductsResponse(mappedProducts, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách sản phẩm.");
                return new ProductsResponse(false, "Không thể tải danh sách sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
