using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.Product.Queries.Products;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.Product.GetProducts
{
    public class GetProductsPresenter : IOutputPort<GetProductsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetProductsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetProductsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetProductsResponseViewModel(response.Products, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
