using Microsoft.Extensions.Localization;
using BizMate.Application.Resources;

namespace BizMate.Public.Message
{
    public static class CommonAppMessageUtils
    {
        public static string NotExist<T>(object value, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON_NOT_EXIST", entity, value?.ToString() ?? "null"];
        }

        public static string AlreadyExist<T>(object value, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON_ALREADY_EXIST", entity, value?.ToString() ?? "null"];
        }

        public static string Invalid<T>(object value, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON_INVALID", entity, value?.ToString() ?? "null"];
        }

        public static string RequiredField(string fieldName, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_REQUIRED_FIELD", fieldName];
        }

        public static string MaxLength(string fieldName, int maxLength, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_MAX_LENGTH", fieldName, maxLength];
        }

        public static string MinLength(string fieldName, int minLength, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_MIN_LENGTH", fieldName, minLength];
        }

        public static string Unauthorized(IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_UNAUTHORIZED"];
        }

        public static string Forbidden(IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_FORBIDDEN"];
        }

        public static string UnexpectedError(IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_UNEXPECTED_ERROR"];
        }

        public static string Conflict(string message, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_CONFLICT", message];
        }

        public static string NotEmpty(string fieldName, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_NOT_EMPTY", fieldName];
        }

        public static string DuplicateData(string fieldName, IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_DUPLICATE", fieldName];
        }

        public static string ConcurrencyConflict(IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys> localizer)
        {
            return localizer["COMMON_CONCURRENCY_CONFLICT"];
        }
    }
}
