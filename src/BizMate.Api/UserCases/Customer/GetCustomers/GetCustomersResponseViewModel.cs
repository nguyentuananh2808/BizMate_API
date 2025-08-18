using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Customer.GetCustomers
{
    public class GetCustomersResponseViewModel
    {
        public IEnumerable<CustomerCoreDto> Customers { get; set; }
        public int TotalCount { get; }
        public GetCustomersResponseViewModel(IEnumerable<CustomerCoreDto> customers, int totalCount)
        {
            Customers = customers;
            TotalCount = totalCount;
        }
    }
}
