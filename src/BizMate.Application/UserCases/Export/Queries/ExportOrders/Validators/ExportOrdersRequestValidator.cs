using BizMate.Application.UserCases.Export.Queries.ExportOrders;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts.Validators
{
    public class ExportOrdersRequestValidator : AbstractValidator<ExportOrdersRequest>
    {
        public ExportOrdersRequestValidator(IStringLocalizer<ExportOrdersRequest> localizer)
        {
            RuleFor(x => x.CustomerId).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DateTo).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
