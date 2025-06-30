using BizMate.Public.Message;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BizMate.Application.Common.Requests.Validators
{
    public class SearchCoreValidator : AbstractValidator<SearchCore>
    {
        public SearchCoreValidator(IStringLocalizer<SearchCore> localizer)
        {
            RuleFor(x => x.KeySearch)
                .MaximumLength(100)
                .WithMessage(localizer[ValidationMessage.LocalizedStrings.MaximumLiability]);
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustGreaterThanOrEqualTo]);
            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage(localizer[ValidationMessage.LocalizedStrings.MustGreaterThanOrEqualTo]);
        }
    }
}
