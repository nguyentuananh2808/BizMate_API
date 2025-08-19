using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using _DealerLevel = BizMate.Domain.Entities.DealerLevel;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel
{
    public class CreateDealerLevelHandler : IRequestHandler<CreateDealerLevelRequest, CreateDealerLevelResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<CreateDealerLevelHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public CreateDealerLevelHandler(
            IAppMessageService messageService,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IDealerLevelRepository dealerLevelRepository,
            QueryFactory db,
            ILogger<CreateDealerLevelHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _DealerLevelRepository = dealerLevelRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<CreateDealerLevelResponse> Handle(CreateDealerLevelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                var name = request.Name.Trim();
                #region check DealerLevel duplicate
                var existingDealerLevel = await _DealerLevelRepository.SearchDealerLevels(
                    storeId,
                    name,
                    _db);

                if (existingDealerLevel.Any())
                {
                    var message = _messageService.DuplicateData(name);
                    _logger.LogWarning(message);
                    return new CreateDealerLevelResponse(false, $"Khách hàng {name} đã tồn tại.");
                }
                #endregion

                #region create new DealerLevel
                // Generate DealerLevel code
                var DealerLevelCode = await _codeGeneratorService.GenerateCodeAsync("#KH");

                var newDealerLevel = new _DealerLevel
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    Name = name,
                    CreatedBy = Guid.Parse(userId),
                };

                await _DealerLevelRepository.AddAsync(newDealerLevel, cancellationToken);
                #endregion

                var dealerLevelDto = _mapper.Map<DealerLevelCoreDto>(newDealerLevel);

                return new CreateDealerLevelResponse(dealerLevelDto, true, "Tạo bảng giá theo đại lý thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo bảng giá theo đại lý.");
                return new CreateDealerLevelResponse(false, "Không thể tạo bảng giá theo đại lý. Vui lòng thử lại.");
            }
        }
    }
}
