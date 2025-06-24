using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.User.Commands.UserRegister.Validators
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator(IStringLocalizer<UserRegisterRequestValidator> localizer)
        {
            RuleFor(x => x.NameStore).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.FullName).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
                    .EmailAddress().WithMessage(localizer[ValidationMessage.LocalizedStrings.InvalidEnumValue]);
        }
    }
}
