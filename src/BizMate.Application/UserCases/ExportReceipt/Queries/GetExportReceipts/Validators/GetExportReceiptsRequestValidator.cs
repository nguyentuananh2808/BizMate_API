using BizMate.Application.Common.Requests.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts.Validators
{
    public class GetExportReceiptsRequestValidator : AbstractValidator<GetExportReceiptsRequest>
    {
        public GetExportReceiptsRequestValidator(IStringLocalizer<GetExportReceiptsRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
        }
    }
}
