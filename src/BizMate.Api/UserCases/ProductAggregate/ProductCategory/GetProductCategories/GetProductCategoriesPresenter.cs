using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategories;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.GetProductCategories
{
    public class GetProductCategoriesPresenter : IOutputPort<GetProductCategoriesResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetProductCategoriesPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetProductCategoriesResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetProductCategoriesResponseViewModel(response.ProductCategories, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
