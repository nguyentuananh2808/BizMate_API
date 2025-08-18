using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer;
using System.Net;

namespace BizMate.Api.UserCases.Customer.CreateCustomer
{
    public class CreateCustomerPresenter : IOutputPort<CreateCustomerResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateCustomerPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateCustomerResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new CreateCustomerResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
