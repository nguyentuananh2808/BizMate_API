using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.Customer.Commands.DeleteCustomer
{
    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerRequest, DeleteCustomerResponse>
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteCustomerHandler> _logger;

        #region constructor
        public DeleteCustomerHandler(
            IUserSession userSession,
            ICustomerRepository CustomerRepository,
            ILogger<DeleteCustomerHandler> logger)
        {
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
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new DeleteCustomerResponse(false, message);
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
