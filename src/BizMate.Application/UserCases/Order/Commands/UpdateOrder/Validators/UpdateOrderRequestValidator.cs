using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder.Validators
{
    public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
    {

        public UpdateOrderRequestValidator(IStringLocalizer<UpdateOrderRequestValidator> localizer)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.DeliveryAddress).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.CustomerPhone).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.CustomerType).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Details).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            // Validate từng chi tiết
            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(d => d.ProductId)
                    .NotEmpty()
                    .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);

                detail.RuleFor(d => d.Quantity)
                    .GreaterThan(0)
                    .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);


            });
        }
    }
}