using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Order.Queries.GetOrders;
using System.Net;

namespace BizMate.Api.UserCases.Order.GetOrders
{
    public class GetOrdersPresenter : IOutputPort<GetOrdersResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetOrdersPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetOrdersResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetOrdersResponseViewModel(response.Orders, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
