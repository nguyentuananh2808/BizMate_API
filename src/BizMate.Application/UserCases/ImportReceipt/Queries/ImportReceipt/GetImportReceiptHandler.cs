using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt
{
    public class GetImportReceiptHandler : IRequestHandler<GetImportReceiptRequest, GetImportReceiptResponse>
    {
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly ILogger<GetImportReceiptHandler> _logger;
        private readonly IMapper _mapper;

        public GetImportReceiptHandler(
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<GetImportReceiptHandler> logger,
            IMapper mapper)
        {
            _importReceiptRepository = ImportReceiptRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GetImportReceiptResponse> Handle(GetImportReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _importReceiptRepository.GetByIdAsync(request.Id, cancellationToken);

            if (receipt == null)
            {
                var message = ValidationMessage.LocalizedStrings.DataNotExist;
                _logger.LogWarning(message);
                return new GetImportReceiptResponse(false, message);
            }

            var result = _mapper.Map<ImportReceiptCoreDto>(receipt);

            return new GetImportReceiptResponse(result);
        }
    }
}
