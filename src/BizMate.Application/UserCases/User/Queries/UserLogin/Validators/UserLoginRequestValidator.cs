using BizMate.Application.UserCases.User.Queries.UserLogin;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.User.Queries.UserLogin.Validators
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator(IStringLocalizer<UserLoginRequestValidator> localizer) 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .EmailAddress().WithMessage(localizer[ValidationMessage.LocalizedStrings.InvalidEnumValue]);
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                .MinimumLength(8).WithMessage(localizer[ValidationMessage.LocalizedStrings.MustHaveMinLength])
                .Matches(@"[A-Z]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainUppercase])
                .Matches(@"[a-z]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainLowercase])
                .Matches(@"\d").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainDigit])
                .Matches(@"[\W_]").WithMessage(localizer[ValidationMessage.LocalizedStrings.MustContainSpecialChar]);
        }
    }
}
