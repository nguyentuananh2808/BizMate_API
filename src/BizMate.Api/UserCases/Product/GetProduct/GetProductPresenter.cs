using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Product.Queries.Product;
using System.Net;

namespace BizMate.Api.UserCases.Product.GetProduct
{
    public class GetProductPresenter : IOutputPort<GetProductResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetProductPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetProductResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetProductResponseViewModel(response.Product))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
