using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Product.Commands.CreateProduct;
using System.Net;

namespace BizMate.Api.UserCases.Product.CreateProduct
{
    public class ProductPresenter : IOutputPort<CreateProductResponse>
    {
        public JsonContentResult ContentResult { get; }

        public ProductPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateProductResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new ProductResponseViewModel(response.Prodduct))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
