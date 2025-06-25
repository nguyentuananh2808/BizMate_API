using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class GetProductsHandler : IRequestHandler<GetProductsRequest, GetProductsResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductsHandler> _logger;

        #region constructor
        public GetProductsHandler(
            IProductRepository productRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetProductsHandler> logger)
        {
            _productRepository = productRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetProductsResponse> Handle(GetProductsRequest request, CancellationToken cancellationToken)
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
                return new GetProductsResponse(mappedProducts, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách sản phẩm.");
                return new GetProductsResponse(false, "Không thể tải danh sách sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
