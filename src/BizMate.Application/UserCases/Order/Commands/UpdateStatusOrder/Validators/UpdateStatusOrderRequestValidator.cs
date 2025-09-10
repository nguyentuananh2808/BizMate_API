using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder.Validators
{
    public class UpdateStatusOrderRequestValidator : AbstractValidator<UpdateStatusOrderRequest>
    {
        public UpdateStatusOrderRequestValidator(IStringLocalizer<UpdateStatusOrderRequestValidator> localizer)
        {
            RuleFor(x => x.StatusId).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
