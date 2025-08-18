using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Customer.Queries.Customer
{
    public class GetCustomerResponse : BaseResponse
    {
        public CustomerCoreDto Customer { get; }
        [JsonConstructor]
        public GetCustomerResponse(CustomerCoreDto customer, bool success = true) : base(success)
        {
            Customer = customer;
        }
        public GetCustomerResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
