using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer.Validators
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator(IStringLocalizer<CreateCustomerRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Address).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
        }
    }
}
