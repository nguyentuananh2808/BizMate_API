using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.CreateProductCategory
{
    public class CreateProductCategoryPresenter : IOutputPort<CreateProductCategoryResponse>
    {
        public JsonContentResult ContentResult { get; }
        public CreateProductCategoryPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateProductCategoryResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new CreateProductCategoryResponseViewModel(response.ProductCategoryCoreDto))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
