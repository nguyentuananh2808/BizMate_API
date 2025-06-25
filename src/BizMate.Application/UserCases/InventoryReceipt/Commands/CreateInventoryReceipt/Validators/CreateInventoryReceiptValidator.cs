using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;
using BizMate.Application.Common.Dto.Identity;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt.Validators
{
    public class CreateInventoryReceiptValidator : AbstractValidator<CreateInventoryReceiptRequest>
    {
        public CreateInventoryReceiptValidator(IStringLocalizer<CreateInventoryReceiptRequest> localizer)
        {
            RuleFor(x => x.Type).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
            .Must(unit => Enum.IsDefined(typeof(InventoryType), unit))
            .WithMessage(localizer["Loại phiếu không hợp lệ"]); ;
        }
    }
}
