using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategory;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategory
{
    public class GetProductCategoryPresenter : IOutputPort<GetProductCategoryResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetProductCategoryPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetProductCategoryResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetProductCategoryResponseViewModel(response.ProductCategory))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
