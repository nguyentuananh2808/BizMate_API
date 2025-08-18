using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.Customer.Queries.Customer
{
    public class GetCustomerHandler : IRequestHandler<GetCustomerRequest, GetCustomerResponse>
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomerHandler> _logger;

        #region constructor
        public GetCustomerHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<GetCustomerHandler> logger)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        public async Task<GetCustomerResponse> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var Customer = await _CustomerRepository.GetByIdAsync(request.Id, cancellationToken);

                var mappedCustomer = _mapper.Map<CustomerCoreDto>(Customer);
                return new GetCustomerResponse(mappedCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn khách hàng.");
                return new GetCustomerResponse(false, "Không thể tải khách hàng. Vui lòng thử lại.");
            }
        }
    }
}
