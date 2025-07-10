using BizMate.Application.UserCases.ProductAggregate.Product.Commands.CreateProduct.Validators;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory.Validators
{
    public class CreateProductCategoryRequestValidator : AbstractValidator<CreateProductCategoryRequest>
    {
        public CreateProductCategoryRequestValidator(IStringLocalizer<CreateProductRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
