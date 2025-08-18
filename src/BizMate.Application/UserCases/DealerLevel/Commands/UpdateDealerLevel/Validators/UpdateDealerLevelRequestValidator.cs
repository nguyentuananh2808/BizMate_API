using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel.Validators
{
    public class UpdateDealerLevelRequestValidator : AbstractValidator<UpdateDealerLevelRequest>
    {
        public UpdateDealerLevelRequestValidator(IStringLocalizer<UpdateDealerLevelRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
