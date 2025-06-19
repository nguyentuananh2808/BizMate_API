using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BizMate.Application.Common.Responses
{
    public abstract class BaseResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public IEnumerable<Error> Errors { get; protected set; }

        protected BaseResponse(bool success, string message = null, IEnumerable<Error> errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }
    }

}
