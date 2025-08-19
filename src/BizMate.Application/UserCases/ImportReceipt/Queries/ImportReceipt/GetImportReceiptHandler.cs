using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt
{
    public class GetImportReceiptHandler : IRequestHandler<GetImportReceiptRequest, GetImportReceiptResponse>
    {
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly ILogger<GetImportReceiptHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetImportReceiptHandler> _localizer;

        public GetImportReceiptHandler(
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<GetImportReceiptHandler> logger,
            IMapper mapper,
            IStringLocalizer<GetImportReceiptHandler> localizer)
        {
            _importReceiptRepository = ImportReceiptRepository;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<GetImportReceiptResponse> Handle(GetImportReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _importReceiptRepository.GetByIdAsync(request.Id);

            if (receipt == null)
            {
                var message = _localizer["Không tìm thấy phiếu nhập."];
                _logger.LogWarning(message);
                return new GetImportReceiptResponse(false, message);
            }

            var result = _mapper.Map<GetImportReceiptResponse>(receipt);

            return result;
        }
    }
}
