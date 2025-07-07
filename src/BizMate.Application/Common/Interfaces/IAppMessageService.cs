using Microsoft.Extensions.Localization;

namespace BizMate.Application.Common.Interfaces
{
    public interface IAppMessageService
    {
        string NotExist(object value, IStringLocalizer<Resources.MessageUtils> _localizer);
        string AlreadyExist(object value, IStringLocalizer<Resources.MessageUtils> _localizer);
        string Invalid(object value, IStringLocalizer<Resources.MessageUtils> _localizer);
        string RequiredField(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer);
        string MaxLength(string fieldName, int maxLength, IStringLocalizer<Resources.MessageUtils> _localizer);
        string MinLength(string fieldName, int minLength, IStringLocalizer<Resources.MessageUtils> _localizer);
        string Unauthorized(IStringLocalizer<Resources.MessageUtils> _localizer);
        string Forbidden(IStringLocalizer<Resources.MessageUtils> _localizer);
        string UnexpectedError(IStringLocalizer<Resources.MessageUtils> _localizer);
        string Conflict(string message, IStringLocalizer<Resources.MessageUtils> _localizer);
        string NotEmpty(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer);
        string DuplicateData(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer);
        string ConcurrencyConflict(IStringLocalizer<Resources.MessageUtils> _localizer);
    }
}
