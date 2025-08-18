using BizMate.Application.Common.Requests.Validators;
using BizMate.Application.UserCases.ProductAggregate.Product.Queries.Products;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels.Validators
{
    public class GetDealerLevelsRequestValidator : AbstractValidator<GetProductsRequest>
    {
        public GetDealerLevelsRequestValidator(IStringLocalizer<GetProductsRequest> localizer)
        {
            Include(new SearchCoreValidator(localizer));
        }
    }
}
