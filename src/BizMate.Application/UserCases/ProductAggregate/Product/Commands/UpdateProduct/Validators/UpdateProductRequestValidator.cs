using BizMate.Application.Common.Enums;
using BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct.Validators
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator(IStringLocalizer<UpdateProductRequestValidator> localizer)
        {
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Unit).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
            .Must(unit => Enum.IsDefined(typeof(ProductUnit), unit))
            .WithMessage(localizer["Đơn vị sản phẩm không hợp lệ"]); ;
        }
    }
}
