using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserPassword;
using BizMate.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BizMate.Api.UserCases.User.UserPassword
{
    public sealed class ForgotPasswordPresenter : IOutputPort<ForgotPasswordResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(ForgotPasswordResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success
                ? HttpStatusCode.OK
                : HttpStatusCode.BadRequest);

            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                response.Success
                    ? new
                    {
                        response.Success,
                        response.Message,
                        response.Email,
                        response.OtpExpiredAt
                    }
                    : response);
        }
    }

    public sealed class ResetPasswordPresenter : IOutputPort<ResetPasswordResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(ResetPasswordResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success
                ? HttpStatusCode.OK
                : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(response);
        }
    }

    [Route(ApiNameConstants.ApiV1 + ApiNameConstants.Authentication)]
    [ApiController]
    public sealed class UserPasswordController(
        IMediator mediator,
        ForgotPasswordPresenter forgotPasswordPresenter,
        ResetPasswordPresenter resetPasswordPresenter) : ControllerBase
    {
        [HttpPost(ApiNameConstants.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            forgotPasswordPresenter.Handle(response);
            return forgotPasswordPresenter.ContentResult;
        }

        [HttpPost(ApiNameConstants.ResetPassword)]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            resetPasswordPresenter.Handle(response);
            return resetPasswordPresenter.ContentResult;
        }
    }
}
