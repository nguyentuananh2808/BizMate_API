using BizMate.Application.Common.Dto.Identity;
using BizMate.Application.Common.Interfaces;
using MediatR;

namespace BizMate.Application.Common.UploadImage
{
    public class UploadImageHandler : IRequestHandler<UploadImageCommand, UploadImageResponse>
    {
        private readonly IImageUploader _uploader;

        public UploadImageHandler(IImageUploader uploader)
        {
            _uploader = uploader;
        }

        public async Task<UploadImageResponse> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _uploader.UploadImageAsync(request.File);
                return new UploadImageResponse(result, true, "Tải hình ảnh thành công.");
            }
            catch (Exception ex)
            {
                return new UploadImageResponse(false, $"Tải hình ảnh thất bại: {ex.Message}");
            }
        }

    }
}

