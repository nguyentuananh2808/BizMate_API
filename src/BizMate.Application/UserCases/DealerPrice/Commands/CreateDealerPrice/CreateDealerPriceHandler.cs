using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using _DealerPrice = BizMate.Domain.Entities.DealerPrice;
using Microsoft.Extensions.Logging;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceHandler : IRequestHandler<CreateDealerPriceRequest, CreateDealerPriceResponse>
    {
        private readonly IDealerPriceRepository _dealerPriceRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<CreateDealerPriceHandler> _logger;

        #region constructor
        public CreateDealerPriceHandler(
            IUserSession userSession,
            IDealerPriceRepository DealerPriceRepository,
            ILogger<CreateDealerPriceHandler> logger)
        {
            _userSession = userSession;
            _dealerPriceRepository = DealerPriceRepository;
            _logger = logger;
        }
        #endregion
        public async Task<CreateDealerPriceResponse> Handle(CreateDealerPriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                #region check DealerPrice duplicate
                var existingDealerPrice = await _dealerPriceRepository.CheckDealerPricesExistAsync(
                    storeId,
                    request.ProductIds,
                    request.DealerLevelId
                    );

                if (existingDealerPrice == false)
                {
                    var message = ValidationMessage.LocalizedStrings.AlreadyExist;
                    _logger.LogWarning(message);
                    return new CreateDealerPriceResponse(false, message);
                }
                #endregion

                #region create new DealerPrice
                var newDealerPrices = request.ProductIds.Select(productId => new _DealerPrice
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    ProductId = productId,
                    DealerLevelId = request.DealerLevelId,
                    CreatedBy = Guid.Parse(userId),
                }).ToList();

                await _dealerPriceRepository.AddRangeAsync(newDealerPrices, cancellationToken);


                #endregion

                return new CreateDealerPriceResponse(true, "Tạo giá đại lý của sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo giá đại lý của sản phẩm.");
                return new CreateDealerPriceResponse(false, "Không thể tạo giá đại lý của sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
