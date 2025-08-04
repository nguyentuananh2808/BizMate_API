using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts
{
    public class GetInventoryReceiptsHandler : IRequestHandler<GetInventoryReceiptsRequest, GetInventoryReceiptsResponse>
    {
        private readonly IInventoryReceiptRepository _inventoryReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetInventoryReceiptsHandler> _logger;

        public GetInventoryReceiptsHandler(
            IInventoryReceiptRepository inventoryReceiptRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetInventoryReceiptsHandler> logger)
        {
            _inventoryReceiptRepository = inventoryReceiptRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetInventoryReceiptsResponse> Handle(GetInventoryReceiptsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (receipts, totalCount) = await _inventoryReceiptRepository.SearchReceiptsWithPaging(
                    storeId: storeId,
                    dateFrom: request.DateFrom,
                    dateTo: request.DateTo,
                    statusCode: request.StatusCode,
                    type: request.Type,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory
                );

                var mapped = _mapper.Map<List<InventoryReceiptCoreDto>>(receipts);

                return new GetInventoryReceiptsResponse(mapped, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách phiếu nhập/xuất kho.");
                return new GetInventoryReceiptsResponse(false, "Không thể tải danh sách phiếu kho. Vui lòng thử lại.");
            }
        }
    }
}
