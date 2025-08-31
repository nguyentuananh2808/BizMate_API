using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice
{
    public class UpdateDealerPriceHandler : IRequestHandler<UpdateDealerPriceRequest, UpdateDealerPriceResponse>
    {
        private readonly IDealerPriceRepository _DealerPriceRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateDealerPriceHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateDealerPriceHandler(
            IUserSession userSession,
            IDealerPriceRepository DealerPriceRepository,
            ILogger<UpdateDealerPriceHandler> logger,
            IMapper mapper)
        {
            _userSession = userSession;
            _DealerPriceRepository = DealerPriceRepository;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateDealerPriceResponse> Handle(UpdateDealerPriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check DealerPrice exist
                var DealerPrice = await _DealerPriceRepository.GetByIdAsync(request.Id);
                if (DealerPrice == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateDealerPriceResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (DealerPrice.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateDealerPriceResponse(false, message);
                }
                #endregion

                #region update data
                DealerPrice.StoreId = storeId;
                DealerPrice.Price = request.Price;
                DealerPrice.UpdatedBy = Guid.Parse(userId);
                DealerPrice.UpdatedDate = DateTime.UtcNow;
                DealerPrice.RowVersion = Guid.NewGuid();

                await _DealerPriceRepository.UpdateAsync(DealerPrice);
                #endregion

                var DealerPriceDto = _mapper.Map<DealerPriceCoreDto>(DealerPrice);

                return new UpdateDealerPriceResponse(DealerPriceDto, true, "Cập nhật Giá sản phẩm của đại lý thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Giá sản phẩm của đại lý.");
                return new UpdateDealerPriceResponse(false, "Không thể cập nhật Giá sản phẩm của đại lý. Vui lòng thử lại.");
            }
        }
    }
}