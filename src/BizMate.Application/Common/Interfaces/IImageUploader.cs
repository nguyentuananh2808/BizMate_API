using Microsoft.AspNetCore.Http;

namespace BizMate.Application.Common.Interfaces
{
    public interface IImageUploader
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
