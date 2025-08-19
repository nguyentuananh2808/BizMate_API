using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts
{
    public class GetImportReceiptsHandler : IRequestHandler<GetImportReceiptsRequest, GetImportReceiptsResponse>
    {
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetImportReceiptsHandler> _logger;

        public GetImportReceiptsHandler(
            IImportReceiptRepository ImportReceiptRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetImportReceiptsHandler> logger)
        {
            _importReceiptRepository = ImportReceiptRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetImportReceiptsResponse> Handle(GetImportReceiptsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (receipts, totalCount) = await _importReceiptRepository.SearchReceiptsWithPaging(
                    storeId: storeId,
                    dateFrom: request.DateFrom,
                    dateTo: request.DateTo,
                    statusCode: request.StatusCode,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory
                );

                var mapped = _mapper.Map<List<ImportReceiptCoreDto>>(receipts);

                return new GetImportReceiptsResponse(mapped, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách phiếu nhập.");
                return new GetImportReceiptsResponse(false, "Không thể tải danh sách phiếu nhập kho. Vui lòng thử lại.");
            }
        }
    }
}
