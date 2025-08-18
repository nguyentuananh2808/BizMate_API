using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Customer.Queries.Customers;
using System.Net;

namespace BizMate.Api.UserCases.Customer.GetCustomers
{
    public class GetCustomersPresenter : IOutputPort<GetCustomersResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetCustomersPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetCustomersResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetCustomersResponseViewModel(response.Customers, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
