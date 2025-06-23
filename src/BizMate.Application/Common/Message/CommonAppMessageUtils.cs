using Microsoft.Extensions.Localization;

namespace BizMate.Public.Message
{
    public static class CommonAppMessageUtils
    {
        public static string NotExist<T>(object value, IStringLocalizer localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON.NOT_EXIST", entity, value?.ToString() ?? "null"];
        }

        public static string AlreadyExist<T>(object value, IStringLocalizer localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON.ALREADY_EXIST", entity, value?.ToString() ?? "null"];
        }

        public static string Invalid<T>(object value, IStringLocalizer localizer)
        {
            var entity = typeof(T).Name;
            return localizer["COMMON.INVALID", entity, value?.ToString() ?? "null"];
        }

        public static string RequiredField(string fieldName, IStringLocalizer localizer)
        {
            return localizer["COMMON.REQUIRED_FIELD", fieldName];
        }

        public static string MaxLength(string fieldName, int maxLength, IStringLocalizer localizer)
        {
            return localizer["COMMON.MAX_LENGTH", fieldName, maxLength];
        }

        public static string MinLength(string fieldName, int minLength, IStringLocalizer localizer)
        {
            return localizer["COMMON.MIN_LENGTH", fieldName, minLength];
        }

        public static string Unauthorized(IStringLocalizer localizer)
        {
            return localizer["COMMON.UNAUTHORIZED"];
        }

        public static string Forbidden(IStringLocalizer localizer)
        {
            return localizer["COMMON.FORBIDDEN"];
        }

        public static string UnexpectedError(IStringLocalizer localizer)
        {
            return localizer["COMMON.UNEXPECTED_ERROR"];
        }

        public static string Conflict(string message, IStringLocalizer localizer)
        {
            return localizer["COMMON.CONFLICT", message];
        }

        public static string NotEmpty(string fieldName, IStringLocalizer localizer)
        {
            return localizer["COMMON.NOT_EMPTY", fieldName];
        }
        public static string DuplicateData(string fieldName, IStringLocalizer localizer)
        {
            return localizer["COMMON.DUPLICATE", fieldName];
        }
    }
}
