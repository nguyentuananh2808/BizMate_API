using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice.Validators
{
    public class UpdateDealerPriceRequestValidator : AbstractValidator<UpdateDealerPriceRequest>
    {
        public UpdateDealerPriceRequestValidator(IStringLocalizer<UpdateDealerPriceRequestValidator> localizer)
        {
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Price).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);

        }
    }
}
