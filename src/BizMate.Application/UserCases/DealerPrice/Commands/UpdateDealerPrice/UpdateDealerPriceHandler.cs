using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice
{
    public class UpdateDealerPriceHandler : IRequestHandler<UpdateDealerPriceRequest, UpdateDealerPriceResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IDealerPriceRepository _DealerPriceRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateDealerPriceHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateDealerPriceHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IDealerPriceRepository DealerPriceRepository,
            QueryFactory db,
            ILogger<UpdateDealerPriceHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _DealerPriceRepository = DealerPriceRepository;
            _db = db;
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
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateDealerPriceResponse(false, "Giá sản phẩm của đại lý không tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (DealerPrice.RowVersion != request.RowVersion)
                    return new UpdateDealerPriceResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
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