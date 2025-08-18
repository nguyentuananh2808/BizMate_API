using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel.Validators
{
    public class CreateDealerLevelRequestValidator : AbstractValidator<CreateDealerLevelRequest>
    {
        public CreateDealerLevelRequestValidator(IStringLocalizer<CreateDealerLevelRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
