using BizMate.Application.Common.Dto.Identity;
using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Product.Commands.CreateProduct.Validators
{
    public class CreateProductRequestValidatior : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidatior(IStringLocalizer localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Quantity).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Unit).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty])
            .Must(unit => Enum.IsDefined(typeof(ProductUnit), unit))
            .WithMessage(localizer["Đơn vị sản phẩm không hợp lệ"]); ;
        }
    }
}
