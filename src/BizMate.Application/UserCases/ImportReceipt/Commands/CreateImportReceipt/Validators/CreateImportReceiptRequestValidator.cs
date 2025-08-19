using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt.Validators
{
    public class CreateImportReceiptRequestValidator : AbstractValidator<CreateImportReceiptRequest>
    {
        public CreateImportReceiptRequestValidator(IStringLocalizer<CreateImportReceiptRequestValidator> localizer)
        {
            RuleFor(x => x.TotalAmount).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Details).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            // Validate từng chi tiết
            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(d => d.ProductId)
                    .NotEmpty()
                    .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);

                detail.RuleFor(d => d.Quantity)
                    .GreaterThan(0)
                    .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);

            });
        }
    }
}
