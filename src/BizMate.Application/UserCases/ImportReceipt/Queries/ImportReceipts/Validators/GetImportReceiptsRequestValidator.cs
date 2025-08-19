using BizMate.Application.Common.Requests.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts.Validators
{
    public class GetImportReceiptsRequestValidator : AbstractValidator<GetImportReceiptsRequest>
    {
        public GetImportReceiptsRequestValidator(IStringLocalizer<GetImportReceiptsRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
        }
    }
}
