using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;
using BizMate.Application.Common.Enums;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt.Validators
{
    public class UpdateInventoryReceiptValidator : AbstractValidator<CreateInventoryReceiptRequest>
    {
        public UpdateInventoryReceiptValidator(IStringLocalizer<CreateInventoryReceiptRequest> localizer)
        {
            RuleFor(x => x.Type).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
            .Must(unit => Enum.IsDefined(typeof(InventoryType), unit))
            .WithMessage(localizer["Loại phiếu không hợp lệ"]); ;
        }
    }
}
