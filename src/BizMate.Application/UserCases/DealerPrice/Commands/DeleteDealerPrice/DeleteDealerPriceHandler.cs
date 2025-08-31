using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.DealerPrice.Commands.DeleteDealerPrice
{
    public class DeleteDealerPriceHandler : IRequestHandler<DeleteDealerPriceRequest, DeleteDealerPriceResponse>
    {
        private readonly IDealerPriceRepository _DealerPriceRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteDealerPriceHandler> _logger;

        #region constructor
        public DeleteDealerPriceHandler(
            IUserSession userSession,
            IDealerPriceRepository DealerPriceRepository,
            ILogger<DeleteDealerPriceHandler> logger)
        {
            _userSession = userSession;
            _DealerPriceRepository = DealerPriceRepository;
            _logger = logger;
        }
        #endregion
        public async Task<DeleteDealerPriceResponse> Handle(DeleteDealerPriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var dealerPrice = await _DealerPriceRepository.GetByIdAsync(request.DealerPriceId, cancellationToken);
                if (dealerPrice == null || dealerPrice.StoreId != storeId)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new DeleteDealerPriceResponse(false, message);
                }

                await _DealerPriceRepository.DeleteAsync(request.DealerPriceId);

                return new DeleteDealerPriceResponse(true, "Xóa giá của sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giá của sản phẩm.");
                return new DeleteDealerPriceResponse(false, "Không thể xóa giá của sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
