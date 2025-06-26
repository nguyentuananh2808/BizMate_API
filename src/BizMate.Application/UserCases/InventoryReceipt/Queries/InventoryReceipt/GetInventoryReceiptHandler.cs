using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt
{
    public class GetInventoryReceiptHandler : IRequestHandler<GetInventoryReceiptRequest, GetInventoryReceiptResponse>
    {
        private readonly IInventoryReceiptRepository _inventoryReceiptRepository;
        private readonly ILogger<GetInventoryReceiptHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetInventoryReceiptHandler> _localizer;

        public GetInventoryReceiptHandler(
            IInventoryReceiptRepository inventoryReceiptRepository,
            ILogger<GetInventoryReceiptHandler> logger,
            IMapper mapper,
            IStringLocalizer<GetInventoryReceiptHandler> localizer)
        {
            _inventoryReceiptRepository = inventoryReceiptRepository;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<GetInventoryReceiptResponse> Handle(GetInventoryReceiptRequest request, CancellationToken cancellationToken)
        {
            var receipt = await _inventoryReceiptRepository.GetByIdAsync(request.Id);

            if (receipt == null)
            {
                var message = _localizer["Không tìm thấy phiếu nhập/xuất kho."];
                _logger.LogWarning(message);
                return new GetInventoryReceiptResponse(false, message);
            }

            var mapped = _mapper.Map<InventoryReceiptCoreDto>(receipt);

            var mappedDetails = _mapper.Map<IEnumerable<Commands.CreateInventoryReceipt.InventoryReceiptDetailDto>>(mapped.InventoryDetailDtos);

            return new GetInventoryReceiptResponse(
                    mapped.InventoryCode,
                    mapped.Date,
                    mapped.Type,
                    mapped.StoreId,
                    mapped.StoreName,
                    mapped.CreatedByUserId,
                    mapped.CreatedByUserName,
                    mapped.SupplierName,
                    mapped.CustomerName,
                    mapped.CustomerPhone,
                    mapped.DeliveryAddress,
                    mapped.Description,
                    mappedDetails,
                    true,
                    "Lấy thành công"
);

        }
    }
}
