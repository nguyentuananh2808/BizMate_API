using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Customer.Queries.Customer;
using System.Net;

namespace BizMate.Api.UserCases.Customer.GetCustomer
{
    public class GetCustomerPresenter : IOutputPort<GetCustomerResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetCustomerPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetCustomerResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetCustomerResponseViewModel(response.Customer))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
