using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizMate.Application.UserCases.Export.Queries.ExportOrders
{
    public class ExportOrdersHandler : IRequestHandler<ExportOrdersRequest, ExportOrdersResponse>
    {
        private readonly IExportRepository _exportRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetExportReceiptsHandler> _logger;

        public ExportOrdersHandler(
            IExportRepository ExportRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetExportReceiptsHandler> logger)
        {
            _exportRepository = ExportRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ExportOrdersResponse> Handle(ExportOrdersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var exportOrders = await _exportRepository.ExportOrderAsync(
                    storeId: storeId,
                    exportOrder: request,
              cancellationToken: cancellationToken
                );

                var mapped = _mapper.Map<ExportOrderCoreDto>(exportOrders);

                return new ExportOrdersResponse(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách phiếu xuất.");
                return new ExportOrdersResponse(false, "Không thể tải danh sách phiếu xuất. Vui lòng thử lại.");
            }
        }
    }
}
