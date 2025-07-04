using BizMate.Api.Serialization;
using BizMate.Api.UserCases.Product.CreateProduct;
using BizMate.Application.Common.Dto.Identity;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.UploadImage;
using BizMate.Application.UserCases.Product.Commands.CreateProduct;
using System.Net;

namespace BizMate.Api.Extensions.UploadImage
{
    public class UploadImagePresenter : IOutputPort<UploadImageResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UploadImagePresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UploadImageResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UploadImageResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
