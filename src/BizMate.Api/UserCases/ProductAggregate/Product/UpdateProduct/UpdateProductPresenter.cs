using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.Product.UpdateProduct
{
    public class UpdateProductPresenter : IOutputPort<UpdateProductResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateProductPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateProductResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UpdateProductResponseViewModel(response.Prodduct))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
