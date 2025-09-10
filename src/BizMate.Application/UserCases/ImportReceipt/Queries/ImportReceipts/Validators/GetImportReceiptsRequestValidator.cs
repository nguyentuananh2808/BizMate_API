using BizMate.Application.Common.Requests.Validators;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts.Validators
{
    public class GetImportReceiptsRequestValidator : AbstractValidator<GetImportReceiptsRequest>
    {
        public GetImportReceiptsRequestValidator(IStringLocalizer<GetImportReceiptsRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DateTo).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
