using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.UpdateProductCategory
{
    public class UpdateProductCategoryPresenter : IOutputPort<UpdateProductCategoryResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateProductCategoryPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateProductCategoryResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UpdateProductCategoryResponseViewModel(response.ProdductCategory))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
