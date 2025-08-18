using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Application.UserCases.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateCustomerHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateCustomerHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            ICustomerRepository CustomerRepository,
            QueryFactory db,
            ILogger<UpdateCustomerHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _CustomerRepository = CustomerRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateCustomerResponse> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check Customer exist
                var Customer = await _CustomerRepository.GetByIdAsync(request.Id);
                if (Customer == null)
                {
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateCustomerResponse(false, "Sản phẩm không tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (Customer.RowVersion != request.RowVersion)
                    return new UpdateCustomerResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
                #endregion

                #region update data
                Customer.Name = request.Name;
                Customer.Phone = request.Phone;
                Customer.Address = request.Address;
                Customer.StoreId = storeId;
                Customer.UpdatedBy = Guid.Parse(userId);
                Customer.UpdatedDate = DateTime.UtcNow;
                Customer.IsActive = request.IsActive;
                Customer.RowVersion = Guid.NewGuid();

                await _CustomerRepository.UpdateAsync(Customer);
                #endregion

                var CustomerDto = _mapper.Map<CustomerCoreDto>(Customer);

                return new UpdateCustomerResponse(CustomerDto, true, "Cập nhật khách hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khách hàng.");
                return new UpdateCustomerResponse(false, "Không thể cập nhật khách hàng. Vui lòng thử lại.");
            }
        }
    }
}
