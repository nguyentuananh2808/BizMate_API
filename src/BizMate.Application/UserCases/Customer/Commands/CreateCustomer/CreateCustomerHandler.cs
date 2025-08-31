using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using AutoMapper;
using _Customer = BizMate.Domain.Entities.Customer;
using BizMate.Application.Common.Security;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CreateCustomerResponse>
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<CreateCustomerHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public CreateCustomerHandler(
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            ICustomerRepository CustomerRepository,
            QueryFactory db,
            ILogger<CreateCustomerHandler> logger,
            IMapper mapper)
        {
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _CustomerRepository = CustomerRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<CreateCustomerResponse> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                var phone = request.Phone.Trim();
                var name = request.Name.Trim();
                var address = request.Address.Trim();
                #region check Customer duplicate
                var existingCustomer = await _CustomerRepository.SearchCustomers(
                    storeId,
                    phone,
                    name,
                    _db);

                if (existingCustomer.Any())
                {
                    var message = ValidationMessage.LocalizedStrings.AlreadyExist;
                    _logger.LogWarning(message);
                    return new CreateCustomerResponse(false, message);
                }
                #endregion

                #region create new Customer
                // Generate Customer code
                var CustomerCode = await _codeGeneratorService.GenerateCodeAsync("#KH");

                var newCustomer = new _Customer
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    Code = CustomerCode,
                    Name = name,
                    Phone = phone,
                    Address = address,
                    DealerLevelId = request.DealerLevelId,
                    CreatedBy = Guid.Parse(userId),
                };

                await _CustomerRepository.AddAsync(newCustomer, cancellationToken);
                #endregion

                var CustomerDto = _mapper.Map<CustomerCoreDto>(newCustomer);

                return new CreateCustomerResponse(CustomerDto, true, "Tạo khách hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khách hàng.");
                return new CreateCustomerResponse(false, "Không thể tạo khách hàng. Vui lòng thử lại.");
            }
        }
    }
}
