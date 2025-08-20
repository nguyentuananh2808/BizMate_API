using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt.Validators
{
    public class UpdateImportReceiptRequestValidator : AbstractValidator<UpdateImportReceiptRequest>
    {
        public UpdateImportReceiptRequestValidator(IStringLocalizer<UpdateImportReceiptRequestValidator> localizer)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.TotalAmount).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
