using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt.Validators
{
    public class UpdateInventoryReceiptValidator : AbstractValidator<UpdateInventoryReceiptRequest>
    {
        public UpdateInventoryReceiptValidator(IStringLocalizer<UpdateInventoryReceiptRequest> localizer)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
