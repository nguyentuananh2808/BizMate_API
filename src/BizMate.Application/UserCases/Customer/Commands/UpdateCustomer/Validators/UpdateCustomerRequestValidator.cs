using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.UserCases.Customer.Commands.UpdateCustomer.Validators
{
    public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerRequestValidator(IStringLocalizer<UpdateCustomerRequestValidator> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Address).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizer[ValidationMessage.LocalizedStrings.MustNotEmpty]);
           
        }
    }
}
