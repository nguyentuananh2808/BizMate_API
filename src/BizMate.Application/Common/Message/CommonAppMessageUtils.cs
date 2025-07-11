using Microsoft.Extensions.Localization;
using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.Common.Message
{
    public class CommonAppMessageUtils : IAppMessageService
    {
        private readonly IStringLocalizer _localizer;

        public CommonAppMessageUtils(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create("MessageUtils", "BizMate.Application");
        }

        public string NotExist<T>(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            var entity = typeof(T).Name;
            return _localizer["COMMON_NOT_EXIST", entity, value?.ToString() ?? "null"];
        }

        public string AlreadyExist<T>(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            var entity = typeof(T).Name;
            return _localizer["COMMON_ALREADY_EXIST", entity, value?.ToString() ?? "null"];
        }

        public string Invalid<T>(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            var entity = typeof(T).Name;
            return _localizer["COMMON_INVALID", entity, value?.ToString() ?? "null"];
        }

        public string RequiredField(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_REQUIRED_FIELD", fieldName];

        public string MaxLength(string fieldName, int maxLength, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_MAX_LENGTH", fieldName, maxLength];

        public string MinLength(string fieldName, int minLength, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_MIN_LENGTH", fieldName, minLength];

        public string Unauthorized(IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_UNAUTHORIZED"];

        public string Forbidden(IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_FORBIDDEN"];

        public string UnexpectedError(IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_UNEXPECTED_ERROR"];

        public string Conflict(string message, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_CONFLICT", message];

        public string NotEmpty(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_NOT_EMPTY", fieldName];

        public string DuplicateData(string fieldName, IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_DUPLICATE", fieldName];

        public string ConcurrencyConflict(IStringLocalizer<Resources.MessageUtils> _localizer)
            => _localizer["COMMON_CONCURRENCY_CONFLICT"];

        public string NotExist(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            return _localizer["COMMON_NOT_EXIST", value?.ToString() ?? "null"];
        }

        public string AlreadyExist(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            return _localizer["COMMON_ALREADY_EXIST", value?.ToString() ?? "null"];
        }  
        
        public string OtpNotExist(IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            return _localizer["COMMON_NOT_EXIST_OTP"];
        }

        public string OtpInvalid(IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            return _localizer["COMMON_INVALID_OTP"];
        }

        public string Invalid(object value, IStringLocalizer<Resources.MessageUtils> _localizer)
        {
            return _localizer["COMMON_INVALID", value?.ToString() ?? "null"];
        }

    }
}
