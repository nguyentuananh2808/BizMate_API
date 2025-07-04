using BizMate.Application.Common.Dto.Identity;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.Extensions.UploadImage
{
    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Image)]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly UploadImagePresenter _presenter;
        private readonly IMediator _mediator;

        public ImageController(UploadImagePresenter presenter, IMediator mediator)
        {
            _presenter = presenter;
            _mediator = mediator;
        }


        [HttpPost(ApiNameConstants.Upload)]
        public async Task<IActionResult> Upload(UploadImageCommand request)
        {
            var response = await _mediator.Send(request);
            _presenter.Handle(response);
            return _presenter.ContentResult;
        }
    }
}
