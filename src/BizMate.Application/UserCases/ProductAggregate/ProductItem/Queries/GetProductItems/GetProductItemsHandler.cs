using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ProductAggregate.ProductItem.Queries.GetProductItems
{
    public class GetProductItemsHandler : IRequestHandler<GetProductItemsRequest, GetProductItemsResponse>
    {
        private readonly IProductItemRepository _productItemRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<GetProductItemsHandler> _logger;

        public GetProductItemsHandler(
            IProductItemRepository productItemRepository,
            IUserSession userSession,
            ILogger<GetProductItemsHandler> logger)
        {
            _productItemRepository = productItemRepository;
            _userSession = userSession;
            _logger = logger;
        }

        public async Task<GetProductItemsResponse> Handle(
            GetProductItemsRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                ProductItemStatus? status = null;
                if (request.Status.HasValue)
                {
                    if (!Enum.IsDefined(typeof(ProductItemStatus), request.Status.Value))
                        return new GetProductItemsResponse(false, "Trạng thái SN không hợp lệ.");

                    status = (ProductItemStatus)request.Status.Value;
                }

                var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
                var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 200);

                var (items, totalCount) = await _productItemRepository.GetByProductAsync(
                    _userSession.StoreId,
                    request.ProductId,
                    status,
                    request.Keyword,
                    pageIndex,
                    pageSize,
                    cancellationToken);

                var result = items.Select(item => new ProductItemCoreDto
                {
                    Id = item.Id,
                    StoreId = item.StoreId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name,
                    ProductCode = item.Product?.Code,
                    SerialNumber = item.SerialNumber,
                    Status = (int)item.Status,
                    StatusName = item.Status.ToString(),
                    ImportReceiptDetailId = item.ImportReceiptDetailId,
                    OrderDetailId = item.OrderDetailId,
                    SoldAt = item.SoldAt,
                    CreatedDate = item.CreatedDate,
                    UpdatedDate = item.UpdatedDate,
                    RowVersion = item.RowVersion
                }).ToList();

                return new GetProductItemsResponse(result, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách SN.");
                return new GetProductItemsResponse(false, "Không thể tải danh sách SN. Vui lòng thử lại.");
            }
        }
    }
}
