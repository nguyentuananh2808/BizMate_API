using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp.Validators
{
    public class UserVerifyOtpRequestValidator : AbstractValidator<UserVerifyOtpRequest>
    {
        public UserVerifyOtpRequestValidator(IStringLocalizer<UserVerifyOtpRequestValidator> localizer)
        {
            RuleFor(x => x.Otp).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Email)
                      .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                      .EmailAddress().WithMessage(localizer[ValidationMessage.LocalizedStrings.InvalidEnumValue]);
        }
    }
}
