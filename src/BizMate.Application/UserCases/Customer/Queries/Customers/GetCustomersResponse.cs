using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Customer.Queries.Customers
{
    public class GetCustomersResponse : BaseResponse
    {
        public IEnumerable<CustomerCoreDto> Customers { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetCustomersResponse(IEnumerable<CustomerCoreDto> customers, int totalCount, bool success = true) : base(success)
        {
            Customers = customers;
            TotalCount = totalCount;
        }

        public GetCustomersResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
