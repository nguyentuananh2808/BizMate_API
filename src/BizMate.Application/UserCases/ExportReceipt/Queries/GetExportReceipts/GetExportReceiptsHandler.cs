using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts
{
    public class GetExportReceiptsHandler : IRequestHandler<GetExportReceiptsRequest, GetExportReceiptsResponse>
    {
        private readonly IExportReceiptRepository _exportReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetExportReceiptsHandler> _logger;

        public GetExportReceiptsHandler(
            IExportReceiptRepository ExportReceiptRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetExportReceiptsHandler> logger)
        {
            _exportReceiptRepository = ExportReceiptRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetExportReceiptsResponse> Handle(GetExportReceiptsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (receipts, totalCount) = await _exportReceiptRepository.SearchReceiptsWithPaging(
                    storeId: storeId,
                    dateFrom: request.DateFrom,
                    dateTo: request.DateTo,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory
                );

                var mapped = _mapper.Map<List<ExportReceiptCoreDto>>(receipts);

                return new GetExportReceiptsResponse(mapped, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách phiếu nhập.");
                return new GetExportReceiptsResponse(false, "Không thể tải danh sách phiếu nhập kho. Vui lòng thử lại.");
            }
        }
    }
}
