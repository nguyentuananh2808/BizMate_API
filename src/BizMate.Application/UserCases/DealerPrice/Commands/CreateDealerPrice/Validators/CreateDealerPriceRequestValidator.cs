using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice.Validators
{
    public class CreateDealerPriceRequestValidator : AbstractValidator<CreateDealerPriceRequest>
    {
        public CreateDealerPriceRequestValidator(IStringLocalizer<CreateDealerPriceRequestValidator> localizer)
        {
            RuleFor(x => x.ProductIds).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DealerLevelId).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
