using BizMate.Application.Common.Responses;

namespace BizMate.Application.Common.UploadImage
{
    public class UploadImageResponse : BaseResponse
    {
        public string? UrlImage { get; set; }
        public UploadImageResponse(string urlImage, bool success = true, string message = null) : base(success, message)
        {
            UrlImage = urlImage;
        }
        public UploadImageResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
