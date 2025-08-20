using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt.Validators
{
    public class UpdateStatusImportReceiptRequestValidator : AbstractValidator<UpdateStatusImportReceiptRequest>
    {
        public UpdateStatusImportReceiptRequestValidator(IStringLocalizer<UpdateStatusImportReceiptRequestValidator> localizer)
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
