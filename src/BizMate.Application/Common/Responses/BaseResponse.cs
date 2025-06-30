using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BizMate.Application.Common.Responses
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<Error> Errors { get; set; }

        public BaseResponse(bool success, string message = null, IEnumerable<Error> errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }
    }

}
