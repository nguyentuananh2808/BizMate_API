using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResponse>
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateCustomerHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateCustomerHandler(
            IUserSession userSession,
            ICustomerRepository CustomerRepository,
            ILogger<UpdateCustomerHandler> logger,
            IMapper mapper)
        {
            _userSession = userSession;
            _CustomerRepository = CustomerRepository;
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
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateCustomerResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (Customer.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateCustomerResponse(false, message);

                }
                #endregion

                #region update data
                Customer.Name = request.Name.Trim();
                Customer.Phone = request.Phone.Trim();
                Customer.Address = request.Address.Trim();
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
