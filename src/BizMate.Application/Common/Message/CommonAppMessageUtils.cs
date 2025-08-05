using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.Common.Message
{
    public class CommonAppMessageUtils : IAppMessageService
    {
        public string NotExist<T>(object value)
        {
            var entity = typeof(T).Name;
            return $"{entity} với giá trị [{value?.ToString() ?? "null"}] không tồn tại.";
        }

        public string AlreadyExist<T>(object value)
        {
            var entity = typeof(T).Name;
            return $"{entity} với giá trị [{value?.ToString() ?? "null"}] đã tồn tại.";
        }

        public string Invalid<T>(object value)
        {
            var entity = typeof(T).Name;
            return $"{entity} với giá trị [{value?.ToString() ?? "null"}] không hợp lệ.";
        }

        public string RequiredField(string fieldName)
            => $"Trường '{fieldName}' là bắt buộc.";

        public string MaxLength(string fieldName, int maxLength)
            => $"Trường '{fieldName}' không được vượt quá {maxLength} ký tự.";

        public string MinLength(string fieldName, int minLength)
            => $"Trường '{fieldName}' phải có ít nhất {minLength} ký tự.";

        public string Unauthorized()
            => "Bạn chưa được xác thực. Vui lòng đăng nhập lại.";

        public string Forbidden()
            => "Bạn không có quyền truy cập chức năng này.";

        public string UnexpectedError()
            => "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";

        public string Conflict(string message)
            => $"Xung đột dữ liệu: {message}";

        public string NotEmpty(string fieldName)
            => $"Trường '{fieldName}' không được để trống.";

        public string DuplicateData(string fieldName)
            => $"Giá trị trong trường '{fieldName}' đã bị trùng.";

        public string ConcurrencyConflict()
            => "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại và thử lại.";

        public string NotExist(object value)
            => $"Dữ liệu với giá trị [{value?.ToString() ?? "null"}] không tồn tại.";

        public string AlreadyExist(object value)
            => $"Dữ liệu với giá trị [{value?.ToString() ?? "null"}] đã tồn tại.";

        public string OtpNotExist()
            => "Mã OTP không tồn tại hoặc đã hết hạn.";

        public string OtpInvalid()
            => "Mã OTP không hợp lệ.";

        public string Invalid(object value)
            => $"Giá trị [{value?.ToString() ?? "null"}] không hợp lệ.";
    }
}
