using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Product.Queries.Products;
using System.Net;

namespace BizMate.Api.UserCases.Product.Products
{
    public class ProductsPresenter : IOutputPort<ProductsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public ProductsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(ProductsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new ProductsResponseViewModel(response.Products))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
