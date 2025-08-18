using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Customer.Commands.UpdateCustomer;
using System.Net;

namespace BizMate.Api.UserCases.Customer.UpdateCustomer
{
    public class UpdateCustomerPresenter : IOutputPort<UpdateCustomerResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateCustomerPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateCustomerResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UpdateCustomerResponseViewModel(response.Customer))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
