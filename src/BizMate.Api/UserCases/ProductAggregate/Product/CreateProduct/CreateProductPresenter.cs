using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.Product.Commands.CreateProduct;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.Product.CreateProduct
{
    public class CreateProductPresenter : IOutputPort<CreateProductResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateProductPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateProductResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new CreateProductResponseViewModel(response.Product))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
