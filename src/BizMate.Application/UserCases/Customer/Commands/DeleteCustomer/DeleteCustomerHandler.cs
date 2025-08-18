using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.Customer.Commands.DeleteCustomer
{
    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerRequest, DeleteCustomerResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteCustomerHandler> _logger;

        #region constructor
        public DeleteCustomerHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            ICustomerRepository CustomerRepository,
            ILogger<DeleteCustomerHandler> logger)
        {
            _messageService = messageService;
            _userSession = userSession;
            _CustomerRepository = CustomerRepository;
            _logger = logger;
        }
        #endregion
        public async Task<DeleteCustomerResponse> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var Customer = await _CustomerRepository.GetByIdAsync(request.Id);
                if (Customer == null || Customer.StoreId != storeId)
                {
                    var message = _messageService.NotExist(request.Id);
                    _logger.LogWarning(message);
                    return new DeleteCustomerResponse(false, "Khách hàng không tồn tại.");
                }

                await _CustomerRepository.DeleteAsync(request.Id);

                return new DeleteCustomerResponse(true, "Xóa khách hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khách hàng.");
                return new DeleteCustomerResponse(false, "Không thể xóa khách hàng. Vui lòng thử lại.");
            }
        }
    }
}
