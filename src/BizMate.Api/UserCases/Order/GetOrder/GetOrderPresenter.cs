using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Order.Queries.GetOrder;
using System.Net;

namespace BizMate.Api.UserCases.Order.GetOrder
{
    public class GetOrderPresenter : IOutputPort<GetOrderResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetOrderPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetOrderResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetOrderResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
