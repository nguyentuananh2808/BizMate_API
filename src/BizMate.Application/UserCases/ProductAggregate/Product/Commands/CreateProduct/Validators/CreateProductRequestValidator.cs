using BizMate.Application.Common.Enums;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.CreateProduct.Validators
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator(IStringLocalizer<CreateProductRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Unit).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
            .Must(unit => Enum.IsDefined(typeof(ProductUnit), unit))
            .WithMessage(localizer["Đơn vị sản phẩm không hợp lệ"]); ;
        }
    }
}
