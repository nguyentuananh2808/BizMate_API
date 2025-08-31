using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipt
{
    public class GetExportReceiptHandler : IRequestHandler<GetExportReceiptRequest, GetExportReceiptResponse>
    {
        private readonly IExportReceiptRepository _ExportReceiptRepository;
        private readonly ILogger<GetExportReceiptHandler> _logger;
        private readonly IMapper _mapper;

        public GetExportReceiptHandler(
            IExportReceiptRepository ExportReceiptRepository,
            ILogger<GetExportReceiptHandler> logger,
            IMapper mapper)
        {
            _ExportReceiptRepository = ExportReceiptRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetExportReceiptResponse> Handle(GetExportReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _ExportReceiptRepository.GetByIdAsync(request.Id);

            if (receipt == null)
            {
                var message = ValidationMessage.LocalizedStrings.DataNotExist;
                _logger.LogWarning(message);
                return new GetExportReceiptResponse(false, message);
            }

            var result = _mapper.Map<GetExportReceiptResponse>(receipt);

            return result;
        }
    }
}
