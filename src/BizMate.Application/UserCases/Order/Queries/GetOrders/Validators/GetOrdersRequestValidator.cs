using BizMate.Application.Common.Requests.Validators;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Order.Queries.GetOrders.Validators
{
    public class GetOrdersRequestValidator : AbstractValidator<GetOrdersRequest>
    {
        public GetOrdersRequestValidator(IStringLocalizer<GetOrdersRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DateTo).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
