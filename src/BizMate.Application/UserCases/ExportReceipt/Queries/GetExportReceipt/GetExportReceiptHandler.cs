using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipt
{
    public class GetExportReceiptHandler : IRequestHandler<GetExportReceiptRequest, GetExportReceiptResponse>
    {
        private readonly IExportReceiptRepository _ExportReceiptRepository;
        private readonly ILogger<GetExportReceiptHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetExportReceiptHandler> _localizer;

        public GetExportReceiptHandler(
            IExportReceiptRepository ExportReceiptRepository,
            ILogger<GetExportReceiptHandler> logger,
            IMapper mapper,
            IStringLocalizer<GetExportReceiptHandler> localizer)
        {
            _ExportReceiptRepository = ExportReceiptRepository;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<GetExportReceiptResponse> Handle(GetExportReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _ExportReceiptRepository.GetByIdAsync(request.Id);

            if (receipt == null)
            {
                var message = _localizer["Không tìm thấy phiếu xuất."];
                _logger.LogWarning(message);
                return new GetExportReceiptResponse(false, message);
            }

            var result = _mapper.Map<GetExportReceiptResponse>(receipt);

            return result;
        }
    }
}
