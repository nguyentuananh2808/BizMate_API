using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using _DealerPrice = BizMate.Domain.Entities.DealerPrice;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceHandler : IRequestHandler<CreateDealerPriceRequest, CreateDealerPriceResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IDealerPriceRepository _DealerPriceRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<CreateDealerPriceHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public CreateDealerPriceHandler(
            IAppMessageService messageService,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IDealerPriceRepository DealerPriceRepository,
            QueryFactory db,
            ILogger<CreateDealerPriceHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _DealerPriceRepository = DealerPriceRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<CreateDealerPriceResponse> Handle(CreateDealerPriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                #region check DealerPrice duplicate
                var existingDealerPrice = await _DealerPriceRepository.CheckDealerPriceExist(
                    storeId,
                    request.ProductId,
                    request.DealerLevelId
                    );

                if (existingDealerPrice != null)
                {
                    //var message = _messageService.DuplicateData(existingDealerPrice);
                    //_logger.LogWarning(message);
                    return new CreateDealerPriceResponse(false, $"Giá của sản phẩm và đại lý đã tồn tại.");
                }
                #endregion

                #region create new DealerPrice

                var newDealerPrice = new _DealerPrice
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    ProductId = request.ProductId,
                    Price = request.Price,
                    DealerLevelId = request.DealerLevelId,
                    CreatedBy = Guid.Parse(userId),
                };

                await _DealerPriceRepository.AddAsync(newDealerPrice, cancellationToken);
                #endregion

                var DealerPriceDto = _mapper.Map<DealerPriceCoreDto>(newDealerPrice);

                return new CreateDealerPriceResponse(DealerPriceDto, true, "Tạo giá đại lý của sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo giá đại lý của sản phẩm.");
                return new CreateDealerPriceResponse(false, "Không thể tạo giá đại lý của sản phẩm. Vui lòng thử lại.");
            }
        }
    }
}
