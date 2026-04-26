using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductItem.Queries.GetProductItems;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductItem.GetProductItems
{
    public class GetProductItemsPresenter : IOutputPort<GetProductItemsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetProductItemsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetProductItemsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetProductItemsResponseViewModel(response.ProductItems, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
