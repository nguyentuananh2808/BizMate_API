using FluentValidation;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt.Validators
{
    public class InventoryReceiptDetailValidator : AbstractValidator<InventoryReceiptDetailDto>
    {
        public InventoryReceiptDetailValidator()
        {
            RuleFor(x => x.Unit)
                .IsInEnum()
                .WithMessage("Đơn vị không hợp lệ.");
        }
    }
}