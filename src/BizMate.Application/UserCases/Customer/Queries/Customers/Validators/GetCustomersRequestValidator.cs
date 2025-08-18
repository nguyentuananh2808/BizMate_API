using BizMate.Application.Common.Requests.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Customer.Queries.Customers.Validators
{
    public class GetCustomersRequestValidator : AbstractValidator<GetCustomersRequest>
    {
        public GetCustomersRequestValidator(IStringLocalizer<GetCustomersRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
        }
    }
}
