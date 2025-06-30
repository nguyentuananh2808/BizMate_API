using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Product.Queries.Product
{
    public class GetProductHandler : IRequestHandler<GetProductRequest, GetProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly QueryFactory _queryFactory;
        private readonly ILogger<GetProductHandler> _logger;

        #region constructor
        public GetProductHandler(
            QueryFactory queryFactory,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<GetProductHandler> logger)
        {
            _queryFactory = queryFactory;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        public async Task<GetProductResponse> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productRepository.GetByIdWithQuantityAsync(request.Id, queryFactory: _queryFactory);

                var mappedProduct = _mapper.Map<ProductCoreDto>(product);
                return new GetProductResponse(mappedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn sản phẩm.");
                return new GetProductResponse(false, "Không thể tải sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
