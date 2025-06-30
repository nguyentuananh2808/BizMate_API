using BizMate.Application.Common.Requests.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Product.Queries.Products.Validators
{
    public class GetProductsRequestValidator : AbstractValidator<GetProductsRequest>
    {
        public GetProductsRequestValidator(IStringLocalizer<GetProductsRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
        }
    }
}
