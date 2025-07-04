using BizMate.Application.Common.UploadImage;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BizMate.Application.Common.Dto.Identity
{
    public class UploadImageCommand : IRequest<UploadImageResponse>
    {
        public IFormFile File { get; set; } = default!;
    }
}
