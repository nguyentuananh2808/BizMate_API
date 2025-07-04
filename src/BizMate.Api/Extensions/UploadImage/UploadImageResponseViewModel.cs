using BizMate.Application.Common.Dto.Identity;
using BizMate.Application.Common.UploadImage;

namespace BizMate.Api.Extensions.UploadImage
{
    public class UploadImageResponseViewModel
    {
        public UploadImageResponse UploadImage { get; set; }
        public UploadImageResponseViewModel(UploadImageResponse uploadImage)
        {
            UploadImage = uploadImage;
        }
    }
}
