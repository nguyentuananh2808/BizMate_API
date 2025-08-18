using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Customer.Commands.DeleteCustomer;
using System.Net;

namespace BizMate.Api.UserCases.Customer.DeleteCustomer
{
    public class DeleteCustomerPresenter : IOutputPort<DeleteCustomerResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteCustomerPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteCustomerResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteCustomerResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
