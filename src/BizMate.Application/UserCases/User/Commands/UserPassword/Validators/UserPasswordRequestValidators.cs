using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.User.Commands.UserPassword.Validators
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IStringLocalizer<ForgotPasswordRequestValidator> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .EmailAddress().WithMessage(localizer[ValidationMessage.LocalizedStrings.InvalidEnumValue]);
        }
    }

    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator(IStringLocalizer<ResetPasswordRequestValidator> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .EmailAddress().WithMessage(localizer[ValidationMessage.LocalizedStrings.InvalidEnumValue]);

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .Length(6).WithMessage(localizer[ValidationMessage.LocalizedStrings.MustHaveMinLength]);

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .MinimumLength(8).WithMessage(localizer[ValidationMessage.LocalizedStrings.MustHaveMinLength])
                .Matches(@"[A-Z]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainUppercase])
                .Matches(@"[a-z]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainLowercase])
                .Matches(@"d").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainDigit])
                .Matches(@"[W_]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainSpecialChar]);
        }
    }
}
