using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory;
using System.Net;

namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.DeleteProductCategory
{
    public class DeleteProductCategoryPresenter : IOutputPort<DeleteProductCategoryResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteProductCategoryPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteProductCategoryResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteProductCategoryResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
