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
