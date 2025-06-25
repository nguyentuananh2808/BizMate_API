using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Product.Commands.DeleteProduct;
using System.Net;

namespace BizMate.Api.UserCases.Product.DeleteProduct
{
    public class DeleteProductPresenter : IOutputPort<DeleteProductResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteProductPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteProductResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteProductResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
